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

        [Header("Settings")]
        [SerializeField] private int enemyCount = 10;
        [SerializeField] private bool timerMode = true;
        [SerializeField] private int timerInSeconds = 120;
        [SerializeField] private GameObject gruntPrefab;
        [SerializeField] private GameObject speederPrefab;
        [SerializeField] private GameObject miniBossPrefab;
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
        private Coroutine _spawnGroupCoroutine;
        private bool _isSpawning;
        private Dictionary<EnemyPoolName, string> enemyPoolNameMap = new Dictionary<EnemyPoolName, string>();
        private Dictionary<EnemyPoolName, GameObject> enemyPrefabMap = new Dictionary<EnemyPoolName, GameObject>();

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
            // Create pools
            ObjectPooler.Instance.CreatePool(GRUNT_POOL_NAME, gruntPrefab, 50, new ObjectPoolOptions(_startingPosition));

            OnTimerStart?.Invoke(_minionTimeRemaining);
        }

        private void Update()
        {
            if(_spawnGroupCoroutine == null && CanSpawnEnemy() && !_isSpawning)
            {
                _spawnGroupCoroutine = StartCoroutine(SpawnEnemyGroup());
            }

            if (_minionTimeRemaining > 0)
            {
                _minionTimeRemaining -= Time.deltaTime;
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
                return _minionTimeRemaining != 0;
            }

            return _enemyRefs.Count < enemyCount;
        }

        private IEnumerator SpawnEnemy(EnemyPoolName enemyPoolName, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            string poolName = enemyPoolNameMap[enemyPoolName];
            GameObject newInstance = ObjectPooler.Instance.GetInstanceFromPool(poolName);
            AddEnemyRef(newInstance);
        }

        private IEnumerator SpawnEnemyGroup()
        {
            _isSpawning = true;
            var group = spawnGroups[_spawnGroupIndex];

            yield return new WaitForSeconds(group.spawnDelay);

            foreach(var enemy in group.enemies)
            {
                SpawnEnemy(enemy.poolName, group.spawnDelayGap);
            }

            _spawnGroupIndex++;
            if (_spawnGroupIndex == spawnGroups.Count)
            {
                _spawnGroupIndex = 0;
            }
            _isSpawning = false;
        }

        private void DespawnEnemy(Enemy enemy)
        {
            RemoveEnemyRef(enemy);
            enemy.transform.position = _startingPosition;
            ObjectPooler.Instance.ReturnToPoolWithDelay(enemy.gameObject, 2);
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