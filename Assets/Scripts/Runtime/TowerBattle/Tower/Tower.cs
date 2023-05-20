using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class Tower : SerializedMonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform rotationPoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firingPoint;
        [SerializeField] private TowerScriptableObject towerData;
        [SerializeField] private GameObject textPrefab;

        
        public int MergeLevel => mergeLevel;
        public TowerScriptableObject TowerData => towerData;

        private GameObject _projectileContainer;
        private Enemy _currentEnemyTarget;
        private List<Enemy> _enemiesInRange;

        // battle props
        [Header("Attributes")]
        [ReadOnly]
        [SerializeField] 
        private int mergeLevel;
        [ReadOnly]
        [SerializeField] 
        private Dictionary<Stat, float> stats = new Dictionary<Stat, float>();

        Coroutine currentCoroutine;
        Quaternion? enemyDir;

        private void OnEnable()
        {
            EnemySpawner.OnEnemiesChanged += UpdateEnemies;
            TowerSpawner.OnTowerEnergyIncrease += TowerEnergyIncrease;
        }

        private void OnDisable()
        {
            EnemySpawner.OnEnemiesChanged -= UpdateEnemies;
        }

        private void Start()
        {
            _projectileContainer = new GameObject("Projectiles");
            _projectileContainer.transform.parent = transform;
            _enemiesInRange = new List<Enemy>();
            InitBoxCollider();
            // DEBUG
            GetComponentInChildren<SpriteRenderer>().color = towerData.DebugColor;
        }

        private void Update()
        {
            if (currentCoroutine == null && _currentEnemyTarget != null)
            {
                currentCoroutine = StartCoroutine(Shoot());
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
            foreach(Stat stat in towerData.Stats.Keys)
            {
                stats[stat] = towerData.GetStat(stat, mergeLevel);
            }
        }

        private IEnumerator Shoot()
        {
            yield return new WaitForSeconds(GetStat(Stat.AttackInterval));

            GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, enemyDir.HasValue ? enemyDir.Value : Quaternion.identity, _projectileContainer.transform);
            Projectile projectileScript = projectileObj.GetComponent<Projectile>();
            projectileScript.SetTarget(_currentEnemyTarget);
            projectileScript.SetDamage(GetStat(Stat.Damage));
            projectileObj.GetComponent<SpriteRenderer>().color = towerData.DebugColor;

            currentCoroutine = null;
        }

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
        }

        public float GetStat(Stat stat)
        {
            if (stats.TryGetValue(stat, out float value))
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