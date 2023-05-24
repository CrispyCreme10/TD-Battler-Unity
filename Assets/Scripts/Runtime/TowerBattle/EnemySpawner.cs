using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class EnemySpawner : MonoBehaviour
    {
        public static Action<IEnumerable<Enemy>> OnEnemiesChanged;
        public static Action<float> OnTimerStart;
        public static Action<float, float> OnTimerChanged;
        public static Action OnTimerEnd;

        const string GRUNT_POOL_NAME = "Grunt";
        const string SPEEDER_POOL_NAME = "Speeder";
        const string MINIBOSS_POOL_NAME = "Miniboss";

        [Header("Settings")]
        [SerializeField] private int enemyCount = 10;
        [SerializeField] private bool timerMode = true;
        [SerializeField] private int timerInSeconds = 120;
        [SerializeField] private GameObject gruntPrefab;
        [SerializeField] private GameObject speederPrefab;
        [SerializeField] private GameObject minibossPrefab;
        [SerializeField] private List<SpawnGroup> spawnGroups;

        [Header("Fixed Delay")]
        [SerializeField] private float initSpawnDelay = 1f;
        [SerializeField] private float delayBtwSpawns = 1f;

        public IEnumerable<Enemy> Enemies => _enemyRefs.Select(go => go.GetComponent<Enemy>());

        private Waypoint _waypoint;
        private float _spawnTimer;
        private List<GameObject> _enemyRefs;
        private bool _isInMinionMode;
        private float _minionTimeRemaining;
        private Vector3 _startingPosition;
        private int _spawnGroupIndex;
        private bool _isSpawning;
        private Dictionary<EnemyPoolName, string> enemyPoolNameMap = new Dictionary<EnemyPoolName, string>();
        private Dictionary<EnemyPoolName, GameObject> enemyPrefabMap = new Dictionary<EnemyPoolName, GameObject>();
        private Coroutine _spawnGroupCoroutine;
        
        private void Awake()
        {
            _waypoint = GetComponent<Waypoint>();
            _spawnTimer = initSpawnDelay;
            _enemyRefs = new List<GameObject>();
            _isInMinionMode = true;
            _minionTimeRemaining = timerInSeconds;
            _startingPosition = _waypoint.GetWaypointPosition(0);
            _spawnGroupIndex = 0;

            // map enemy pool name to enemy prefab
            enemyPoolNameMap[EnemyPoolName.Grunt] = GRUNT_POOL_NAME;
            enemyPrefabMap[EnemyPoolName.Grunt] = gruntPrefab;
            enemyPoolNameMap[EnemyPoolName.Speeder] = SPEEDER_POOL_NAME;
            enemyPrefabMap[EnemyPoolName.Speeder] = speederPrefab;
            enemyPoolNameMap[EnemyPoolName.MiniBoss] = MINIBOSS_POOL_NAME;
            enemyPrefabMap[EnemyPoolName.MiniBoss] = minibossPrefab;

            // Create pools
            ObjectPooler.Instance.CreatePool(GRUNT_POOL_NAME, gruntPrefab, 50, new ObjectPoolOptions(_startingPosition));
            ObjectPooler.Instance.CreatePool(SPEEDER_POOL_NAME, speederPrefab, 20, new ObjectPoolOptions(_startingPosition));
            ObjectPooler.Instance.CreatePool(MINIBOSS_POOL_NAME, minibossPrefab, 10, new ObjectPoolOptions(_startingPosition));
        }

        private void OnEnable()
        {
            Enemy.OnDeath += DespawnEnemy;
            Enemy.OnEndReached += DespawnEnemy;
        }

        private void OnDisable()
        {
            Enemy.OnDeath -= DespawnEnemy;
            Enemy.OnEndReached -= DespawnEnemy;
        }

        private void Start()
        {
            OnTimerStart?.Invoke(_minionTimeRemaining);
        }

        private void Update()
        {
            if (CanSpawnEnemy())
            {
                _minionTimeRemaining -= Time.deltaTime;

                if (_spawnGroupCoroutine == null)
                {
                    _spawnGroupCoroutine = StartCoroutine(SpawnEnemyGroup());
                }

                OnTimerChanged?.Invoke(timerInSeconds, _minionTimeRemaining);
            }
            else
            {
                OnTimerEnd?.Invoke();
                _minionTimeRemaining = 0;
                // spawn boss
                // hide & reset timer
            }
        }

        private bool CanSpawnEnemy()
        {
            if (timerMode)
            {
                return _minionTimeRemaining >= 0;
            }

            return _enemyRefs.Count < enemyCount;
        }

        private IEnumerator SpawnEnemyPair(SpawnPair enemyPair)
        {
            yield return new WaitForSeconds(enemyPair.spawnDelay);
            if (enemyPoolNameMap.TryGetValue(enemyPair.poolName, out string poolName))
            {
                for(int i = 0; i < enemyPair.count; i++)
                {
                    SpawnEnemy(poolName);
                    yield return new WaitForSeconds(enemyPair.spawnDelayGap);
                }                
            }
        }

        private void SpawnEnemy(string poolName)
        {
            GameObject newInstance = ObjectPooler.Instance.GetInstanceFromPool(poolName);
            AddEnemyRef(newInstance);
        }

        private IEnumerator SpawnEnemyGroup()
        {
            var group = spawnGroups[_spawnGroupIndex];
            yield return new WaitForSeconds(group.spawnDelay);
            foreach(var enemyPair in group.enemies)
            {
                yield return SpawnEnemyPair(enemyPair);
            }

            _spawnGroupIndex++;
            if (_spawnGroupIndex == spawnGroups.Count)
            {
                _spawnGroupIndex = 0;
            }

            _spawnGroupCoroutine = null;
        }

        private void DespawnEnemy(Enemy enemy)
        {
            StartCoroutine(DespawnEnemyWithDelay(enemy, 1f));
        }

        private IEnumerator DespawnEnemyWithDelay(Enemy enemy, float delay)
        {
            RemoveEnemyRef(enemy);
            ObjectPooler.Instance.ReturnToPool(enemy.gameObject);
            yield return new WaitForSeconds(delay);
            enemy.transform.position = _startingPosition;
        }

        private void AddEnemyRef(GameObject enemyGO)
        {
            if (!_enemyRefs.Contains(enemyGO))
            {
                _enemyRefs.Add(enemyGO);
                OnEnemiesChanged?.Invoke(_enemyRefs.Select(go => go.GetComponent<Enemy>()));
            }
        }

        private void RemoveEnemyRef(Enemy enemy)
        {
            if (_enemyRefs.Contains(enemy.gameObject))
            {
                _enemyRefs.Remove(enemy.gameObject);
                OnEnemiesChanged?.Invoke(_enemyRefs.Select(go => go.GetComponent<Enemy>()));
            }
        }
    }

    [Serializable]
    public class SpawnGroup
    {
        public float spawnDelay = 1f;
        public float spawnDelayGap = 0.5f;
        public List<SpawnPair> enemies;
    }

    [Serializable]
    public struct SpawnPair
    {
        public float spawnFrequency;
        public float spawnDelay;
        public float spawnDelayGap;
        public int count;
        public EnemyPoolName poolName;
    }

    public enum EnemyPoolName
    {
        Grunt,
        Speeder,
        MiniBoss
    }
}