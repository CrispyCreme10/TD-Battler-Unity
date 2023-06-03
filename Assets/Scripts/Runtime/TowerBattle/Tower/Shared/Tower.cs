using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace TDBattler.Runtime
{
    // Tower.cs -> responsible for setting + performing generic Tower things
    //  Damage
    //  Attack Interval
    //  Hero Energy Replenishment
    //  Updating Static Stats
    //  Spawning Projectile
    //  Animations
    public abstract class Tower : SerializedMonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Transform rotationPoint;
        [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected Transform firingPoint;
        [SerializeField] protected TowerScriptableObject towerData;
        [SerializeField] protected GameObject textPrefab;
        [SerializeField] protected Animator anim;

        public int MergeLevel => mergeLevel;
        public int EnergyLevel => energyLevel;
        public TowerScriptableObject TowerData => towerData;

        protected GameObject _projectileContainer;

        protected List<Projectile> _projectilesTargeting = new List<Projectile>();

        // battle props
        [Header("Shared Attributes")]
        [ReadOnly]
        [SerializeField]
        protected int mergeLevel;
        [ReadOnly]
        [SerializeField]
        protected int energyLevel;
        [ReadOnly]
        [SerializeField]
        protected Enemy _currentEnemyTarget;
        [ReadOnly]
        [SerializeField]
        protected Dictionary<StatType, float> statMap = new Dictionary<StatType, float>();
        [ReadOnly]
        [SerializeField]
        protected List<Enemy> _enemiesInRange;
        [ReadOnly]
        [SerializeField]
        [ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElement", OnEndListElementGUI = "EndDrawListElement")]
        protected List<EnemyDistance> _enemiesDistance;

        protected Coroutine currentCoroutine;
        protected Quaternion? enemyDir;

        [Serializable]
        protected struct EnemyDistance
        {
            public float Distance;
            public string Name;
        }

        private void Awake()
        {
            _projectileContainer = new GameObject("Projectiles");
            _projectileContainer.transform.parent = transform;
            _enemiesInRange = new List<Enemy>();
            InitBoxCollider();
            RefreshStats();
            // DEBUG
            GetComponentInChildren<SpriteRenderer>().color = towerData.DebugColor;
        }

        private void OnEnable()
        {
            EnemySpawner.OnEnemiesChanged += UpdateEnemies;

            anim.SetBool("CanShoot", true);
        }

        protected void OnEnableBase()
        {
            EnemySpawner.OnEnemiesChanged += UpdateEnemies;

            anim.SetBool("CanShoot", true);
        }

        private void OnDisable()
        {
            EnemySpawner.OnEnemiesChanged -= UpdateEnemies;

            anim.SetBool("CanShoot", false);
        }

        protected void OnDisableBase()
        {
            EnemySpawner.OnEnemiesChanged -= UpdateEnemies;

            anim.SetBool("CanShoot", false);
        }

        private void Update()
        {
            if (currentCoroutine == null && _currentEnemyTarget != null)
            {
                currentCoroutine = StartCoroutine(Attack());

                enemyDir = RotateTowardsTarget();
            }

            SetCurrentEnemyTarget();
        }

        private void InitBoxCollider()
        {
            Waypoint waypoint = GameObject.Find("EnemySpawner").GetComponent<Waypoint>();
            BoxCollider2D boxCollider = GetComponentInChildren<BoxCollider2D>();
            var ((width, height), center) = waypoint.GetWaypointsBounds();
            Vector3 offset = center - transform.position;
            boxCollider.offset = new Vector2(offset.x / transform.localScale.x, offset.y / transform.localScale.y);
            boxCollider.size = new Vector2(width / transform.localScale.x, height / transform.localScale.y);
        }

        private void InitDebugText()
        {
            var go = Instantiate(textPrefab, transform.position, Quaternion.identity, transform);
            go.GetComponent<TextMesh>().text = mergeLevel.ToString();
        }

        public void SetMergeLevel(int level)
        {
            mergeLevel = level;
            RefreshStats();
            InitDebugText();
        }

        public void SetEnergyLevel(int level)
        {
            energyLevel = level;
            RefreshStats();
        }

        #region General Methods
        
        public void AddProjectile(Projectile projectile)
        {
            _projectilesTargeting.Add(projectile);
        }

        public void RemoveProjectile(Projectile projectile)
        {
            _projectilesTargeting.Remove(projectile);
        }

        protected bool NoEnemyTarget()
        {
            return _enemiesInRange.Count <= 0 || _currentEnemyTarget != null && !_currentEnemyTarget.isActiveAndEnabled;
        }

        protected Enemy GetFirstEnemy()
        {
            if (_enemiesInRange.Count == 0) return null;
            var orderedEnemies = _enemiesInRange.OrderByDescending(e => e.DistanceTraveled);
            _enemiesDistance = orderedEnemies.Select(e => new EnemyDistance{ Distance = e.DistanceTraveled, Name = e.name}).ToList();
            return orderedEnemies.First();
        }

        protected Enemy GetRandomEnemy()
        {
            if (_enemiesInRange.Count == 0) return null;
            return _enemiesInRange[(int)Mathf.Round(UnityEngine.Random.value) * (_enemiesInRange.Count - 1)];
        }

        protected Quaternion? RotateTowardsTarget()
        {
            if (_currentEnemyTarget == null)
            {
                return null;
            }

            float angle = Mathf.Atan2(_currentEnemyTarget.transform.position.y - transform.position.y, _currentEnemyTarget.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            rotationPoint.rotation = targetRotation;

            return targetRotation;
        }

        public void UpdateEnemies(IEnumerable<Enemy> enemies)
        {
            // just get the enemies that are relevant
            _enemiesInRange = enemies.ToList();
            bool enemiesInRange = _enemiesInRange.Count > 0;
            anim.SetBool("CanShoot", enemiesInRange);
        }

        public float GetStat(StatType stat)
        {
            if (statMap.TryGetValue(stat, out float value))
            {
                return value;
            }

            Debug.LogError($"No stat value found for {stat} on {this.name}");
            return 0;
        }

        public void IncreaseTowerEnergy()
        {
            energyLevel++;
            RefreshStats();
        }

        private IEnumerator ApplyStun(float duration)
        {
            gameObject.SetActive(false);
            yield return new WaitForSeconds(duration);
            gameObject.SetActive(true);
        }

        private void BeginDrawListElement(int index)
        {
            SirenixEditorGUI.BeginBox(this._enemiesDistance[index].Name);
        }

        private void EndDrawListElement(int index)
        {
            SirenixEditorGUI.EndBox();
        }
        #endregion

        #region Custom Hooks
        protected void OnCollisionEnter2D(Collision2D other)
        {
            var projectile = other.gameObject.GetComponent<Projectile>();
            if (projectile != null && _projectilesTargeting.Contains(projectile))
            {
                // apply effect
                StartCoroutine(ApplyStun(3f));
                RemoveProjectile(projectile);
            }
        }

        protected abstract void SetCurrentEnemyTarget();

        protected virtual void RefreshStats()
        {
            foreach (Stat stat in towerData.Stats)
            {
                statMap[stat.Type] = stat.GetValueModified(mergeLevel, energyLevel, towerData.PermanentLevel);
            }
        }

        protected virtual IEnumerator Attack()
        {
            var attackInterval = GetStat(StatType.AttackInterval);
            yield return new WaitForSeconds(attackInterval);

            anim.speed = 1 / attackInterval;

            GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, enemyDir.HasValue ? enemyDir.Value : Quaternion.identity, _projectileContainer.transform);
            Projectile projectileScript = projectileObj.GetComponent<Projectile>();
            projectileScript.SetSourceObjectName(towerData.Name);
            projectileScript.SetTarget(_currentEnemyTarget?.gameObject);
            projectileScript.SetDamage(GetStat(StatType.Damage));
            projectileObj.GetComponent<SpriteRenderer>().color = towerData.DebugColor;
            projectileObj.name = $"{name} - {projectileObj.name}";

            if (GenerateEnemyDebuff(out EnemyDebuff enemyDebuff))
            {
                projectileScript.EnemyDebuff = enemyDebuff;
            }

            if (_currentEnemyTarget != null)
            {
                _currentEnemyTarget.AddProjectile(projectileScript);
            }

            currentCoroutine = null;
        }

        public virtual void OnMerge(TowerBattleManager towerBattleManager)
        {
            // provides optional override to derived classes
        }

        public virtual bool GenerateEnemyDebuff(out EnemyDebuff enemyDebuff)
        {
            enemyDebuff = null;
            return false;
        }

        #endregion
    }


    [Serializable]
    public class Stat
    {
        [SerializeField] private StatType type;
        [SerializeField] private float value;
        [SerializeField] private List<StatModifier> modifiers;
        [SerializeField] private StatLevels statLevels;

        public StatType Type => type;

        public float GetValue(int mergeLevel, int energyLevel, int permanentLevel)
        {
            int mergeIndex = mergeLevel - 2;
            int energyIndex = energyLevel - 2;
            int permanentIndex = permanentLevel - 2;

            return value + 
                statLevels.GetMergeValue(mergeIndex) +
                statLevels.GetEnergyValue(energyIndex) +
                statLevels.GetPermanentValue(permanentIndex);
        }

        public float GetValueModified(int mergeLevel, int energyLevel, int permanentLevel)
        {
            int mergeIndex = mergeLevel - 2;
            int energyIndex = energyLevel - 2;
            int permanentIndex = permanentLevel - 2;

            float baseValue = value + 
                statLevels.GetMergeValue(mergeIndex) +
                statLevels.GetEnergyValue(energyIndex) +
                statLevels.GetPermanentValue(permanentIndex);

            return modifiers.Aggregate(baseValue, (total, mod) =>
            {
                float modVal = mod.GetValueAtLevels(mergeIndex, energyIndex, permanentIndex) * (mod.IncreasesValue ? 1 : -1);
                return total * (1 + modVal);
            });
        }
    }

    [Serializable]
    public struct StatModifier
    {
        [SerializeField] private StatModifierType type;
        [SerializeField] private float value;
        [SerializeField] private bool increasesValue;
        [SerializeField] private StatLevels statModifierLevels;

        public bool IncreasesValue => increasesValue;

        public float GetValueAtLevels(int mergeIndex, int energyIndex, int permanentIndex)
        {
            return value + statModifierLevels.GetMergeValue(mergeIndex) +
                statModifierLevels.GetEnergyValue(energyIndex) +
                statModifierLevels.GetPermanentValue(permanentIndex);
        }
    }

    [Serializable]
    public class StatLevels
    {
        const int MAX_MERGE_LEVEL = 7;
        const int MAX_ENERGY_LEVEL = 5;
        const int MAX_PERMANENT_LEVEL = 15;

        [InlineButton("ApplyMergePattern", SdfIconType.Check, "")]
        [LabelText("Pattern")]
        [SerializeField]
        private float mergePatternValue;

        [ListDrawerSettings(OnTitleBarGUI = "DrawMergeButtons")]
        [RequiredListLength(0, MAX_MERGE_LEVEL - 1)]
        public List<float> MergeLevels;

        [Space]
        [InlineButton("ApplyEnergyPattern", SdfIconType.Check, "")]
        [LabelText("Pattern")]
        [SerializeField]
        private float energyPatternValue;

        [ListDrawerSettings(OnTitleBarGUI = "DrawEnergyButtons")]
        [RequiredListLength(0, MAX_ENERGY_LEVEL - 1)]
        public List<float> EnergyLevels;

        [Space]
        [InlineButton("ApplyPermanentPattern", SdfIconType.Check, "")]
        [LabelText("Pattern")]
        [SerializeField]
        private float permanentPatternValue;

        [ListDrawerSettings(OnTitleBarGUI = "DrawPermanentButtons")]
        [RequiredListLength(0, MAX_PERMANENT_LEVEL - 1)]
        public List<float> PermanentLevels;


        #region Inspector

        private void ApplyMergePattern()
        {
            ApplyPattern(mergePatternValue, ref MergeLevels, MAX_MERGE_LEVEL);
        }

        private void DrawMergeButtons()
        {
            DrawButtons(ref MergeLevels, MAX_MERGE_LEVEL);
        }

        private void ApplyEnergyPattern()
        {
            ApplyPattern(energyPatternValue, ref EnergyLevels, MAX_ENERGY_LEVEL);
        }

        private void DrawEnergyButtons()
        {
            DrawButtons(ref EnergyLevels, MAX_ENERGY_LEVEL);
        }

        private void ApplyPermanentPattern()
        {
            ApplyPattern(permanentPatternValue, ref PermanentLevels, MAX_PERMANENT_LEVEL);
        }

        private void DrawPermanentButtons()
        {
            DrawButtons(ref PermanentLevels, MAX_PERMANENT_LEVEL);
        }

        private void RefreshList(ref List<float> list, int count)
        {
            list = Enumerable.Repeat(0f, count).ToList();
        }

        private void ApplyPattern(float patternValue, ref List<float> list, int maxItems)
        {
            if (patternValue != 0)
            {
                if (list.Count == 0)
                {
                    RefreshList(ref list, maxItems - 1);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = MathF.Round(patternValue * (i + 1), 3);
                }

                mergePatternValue = 0;
                energyPatternValue = 0;
                permanentPatternValue = 0;
            }
        }

        private void DrawButtons(ref List<float> list, int maxItems)
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                RefreshList(ref list, maxItems - 1);
            }

            if (SirenixEditorGUI.ToolbarButton(EditorIcons.X))
            {
                list.Clear();
            }
        }

        #endregion

        public float GetMergeValue(int index)
        {
            return GetValue(index, ref MergeLevels);
        }

        public float GetEnergyValue(int index)
        {
            return GetValue(index, ref EnergyLevels);
        }

        public float GetPermanentValue(int index)
        {
            return GetValue(index, ref PermanentLevels);
        }

        public float GetValue(int index, ref List<float> list)
        {
            if (index < 0 || index >= list.Count)
            {
                return 0;
            }

            return list[index];
        }
    }
}