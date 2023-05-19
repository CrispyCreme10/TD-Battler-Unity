using System;
using System.Collections.Generic;
using System.Linq;
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

        [Header("Fixed Delay")]
        [SerializeField] private float initSpawnDelay = 1f;
        [SerializeField] private float delayBtwSpawns = 1f;

        public IEnumerable<Enemy> Enemies => _enemyRefs.Select(go => go.GetComponent<Enemy>());

        private Waypoint _waypoint;
        private float _spawnTimer;
        private int _enemiesSpawned;
        private List<GameObject> _enemyRefs;
        private bool _isInMinionMode;
        private float _minionTimeRemaining;
        private Vector3 _startingPosition;

        private void Awake()
        {
            _waypoint = GetComponent<Waypoint>();
            _spawnTimer = initSpawnDelay;
            _enemyRefs = new List<GameObject>();
            _isInMinionMode = true;
            _minionTimeRemaining = timerInSeconds;
            _startingPosition = _waypoint.GetWaypointPosition(0);
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
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer < 0)
            {
                _spawnTimer = delayBtwSpawns;
                if (CanSpawnEnemy())
                {
                    _enemiesSpawned++;
                    SpawnEnemy();
                }
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

            return _enemiesSpawned < enemyCount;
        }

        private void SpawnEnemy()
        {
            GameObject newInstance = ObjectPooler.Instance.GetInstanceFromPool(GRUNT_POOL_NAME);
            AddEnemyRef(newInstance);
        }

        private void DespawnEnemy(Enemy enemy)
        {
            RemoveEnemyRef(enemy);
            enemy.transform.position = _startingPosition;
            ObjectPooler.Instance.ReturnToPool(enemy.gameObject);
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
}