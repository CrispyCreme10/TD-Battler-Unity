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

        const string GRUNT_POOL_NAME = "Grunt";
        const string SPEEDER_POOL_NAME = "Speeder";
        const string MINIBOSS_POOL_NAME = "Miniboss";

        [Header("References")]
        [SerializeField] private GameObject gruntPrefab;
        [SerializeField] private GameObject speederPrefab;
        [SerializeField] private GameObject minibossPrefab;


        [Header("Settings")]
        [SerializeField] private List<SpawnGroup> spawnGroups;

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
            _enemyRefs = new List<GameObject>();
            _isInMinionMode = true;
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
            BattleManager.OnMinionWaveUpdate += WaveTimeUpdate;
            Enemy.OnDeath += DespawnEnemy;
            Enemy.OnEndReached += DespawnEnemy;
        }

        private void OnDisable()
        {
            BattleManager.OnMinionWaveUpdate -= WaveTimeUpdate;
            Enemy.OnDeath -= DespawnEnemy;
            Enemy.OnEndReached -= DespawnEnemy;
        }

        private void Start()
        {
             
        }

        private void Update()
        {

        }

        private void WaveTimeUpdate(float waveTimer, float waveTimerRemaining)
        {
            // loop spawn groups
            // foreach create a coroutine that is associated with the group
            foreach (SpawnGroup spawnGroup in spawnGroups)
            {
                if (spawnGroup.coroutine == null)
                {
                    spawnGroup.coroutine = StartCoroutine(SpawnEnemyGroup(spawnGroup));
                }
            }
        }

        private IEnumerator SpawnEnemyPair(SpawnUnit enemyPair)
        {
            yield return new WaitForSeconds(enemyPair.initialDelay);
            if (enemyPoolNameMap.TryGetValue(enemyPair.poolName, out string poolName))
            {
                for(int i = 0; i < enemyPair.count; i++)
                {
                    SpawnEnemy(poolName);
                    yield return new WaitForSeconds(enemyPair.enemyDelayGap);
                }                
            }
        }

        private void SpawnEnemy(string poolName)
        {
            GameObject newInstance = ObjectPooler.Instance.GetInstanceFromPool(poolName);
            newInstance.GetComponent<Enemy>().SpawnInit();
            AddEnemyRef(newInstance);
        }

        private IEnumerator SpawnEnemyGroup(SpawnGroup spawnGroup)
        {
            foreach(var enemyPair in spawnGroup.enemies)
            {
                yield return SpawnEnemyPair(enemyPair);
            }

            _spawnGroupIndex++;
            if (_spawnGroupIndex == spawnGroups.Count)
            {
                _spawnGroupIndex = 0;
            }

            yield return new WaitForSeconds(spawnGroup.unitDelayGap);
            // coroutine = null;
        }

        private void DespawnEnemy(Enemy enemy)
        {
            StartCoroutine(DespawnEnemyWithDelay(enemy, 1f));
        }

        private IEnumerator DespawnEnemyWithDelay(Enemy enemy, float delay)
        {
            ObjectPooler.Instance.ReturnToPool(enemy.gameObject);
            RemoveEnemyRef(enemy);
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
        public float unitDelayGap = 1f;
        public List<SpawnUnit> enemies;
        public Coroutine coroutine;
    }

    [Serializable]
    public struct SpawnUnit
    {
        public float initialDelay;
        public float enemyDelayGap;
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