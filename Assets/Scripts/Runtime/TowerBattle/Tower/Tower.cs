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
    public class Tower : SerializedMonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform rotationPoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firingPoint;
        [SerializeField] private TowerScriptableObject towerData;
        [SerializeField] private GameObject textPrefab;
        [SerializeField] private Animator anim;

        public int MergeLevel => mergeLevel;
        public TowerScriptableObject TowerData => towerData;

        private GameObject _projectileContainer;
        
        private List<Enemy> _enemiesInRange;

        // battle props
        [Header("Attributes")]
        [ReadOnly]
        [SerializeField]
        private int mergeLevel;
        [ReadOnly]
        [SerializeField]
        private Dictionary<StatType, float> statMap = new Dictionary<StatType, float>();
        [ReadOnly]
        [SerializeField]
        private Enemy _currentEnemyTarget;

        Coroutine currentCoroutine;
        Quaternion? enemyDir;

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
            TowerSpawner.OnTowerEnergyIncrease += TowerEnergyIncrease;

            anim.SetBool("CanShoot", true);
        }

        private void OnDisable()
        {
            EnemySpawner.OnEnemiesChanged -= UpdateEnemies;
            TowerSpawner.OnTowerEnergyIncrease += TowerEnergyIncrease;

            anim.SetBool("CanShoot", false);
        }

        private void Update()
        {
            if (currentCoroutine == null && _currentEnemyTarget != null)
            {
                currentCoroutine = StartCoroutine(Attack());
            }

            GetCurrentEnemyTarget();
            enemyDir = RotateTowardsTarget();
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

        private void RefreshStats()
        {
            foreach (Stat stat in towerData.Stats)
            {
                statMap[stat.Type] = stat.GetValueModified(mergeLevel, towerData.EnergyLevel, towerData.PermanentLevel);
            }
        }

        #region General Methods

        protected virtual IEnumerator Attack()
        {
            var attackInterval = GetStat(StatType.AttackInterval);
            yield return new WaitForSeconds(attackInterval);

            anim.speed = 1 / attackInterval;

            GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, enemyDir.HasValue ? enemyDir.Value : Quaternion.identity, _projectileContainer.transform);
            Projectile projectileScript = projectileObj.GetComponent<Projectile>();
            projectileScript.SetTarget(_currentEnemyTarget);
            projectileScript.SetDamage(GetStat(StatType.Damage));
            projectileObj.GetComponent<SpriteRenderer>().color = towerData.DebugColor;

            currentCoroutine = null;
        }

        #endregion

        private void GetCurrentEnemyTarget()
        {
            if (_enemiesInRange.Count <= 0)
            {
                _currentEnemyTarget = null;
                return;
            }

            _currentEnemyTarget = _enemiesInRange[0];
        }

        private Quaternion? RotateTowardsTarget()
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
            anim.SetBool("CanShoot", _enemiesInRange.Count > 0);
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

        private void TowerEnergyIncrease(int index, int energyLevel)
        {
            RefreshStats();
        }
    }
}

[Serializable]
public class Stat
{
    [SerializeField] private StatType type;
    [SerializeField] private float value;
    [SerializeField] private List<StatModifier> modifiers;
    [SerializeField] private StatLevels statLevels;

    public StatType Type => type;

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
    [SerializeField] private bool increasesValue;
    [SerializeField] private StatLevels statModifierLevels;

    public bool IncreasesValue => increasesValue;

    public float GetValueAtLevels(int mergeIndex, int energyIndex, int permanentIndex)
    {
        return statModifierLevels.GetMergeValue(mergeIndex) +
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
                list[i] = MathF.Round(patternValue * (i + 1), 2);
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