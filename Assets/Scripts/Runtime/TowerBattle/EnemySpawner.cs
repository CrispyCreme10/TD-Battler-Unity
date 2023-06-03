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
        const string NINJA_POOL_NAME = "Ninja";

        [Header("References")]
        [SerializeField] private GameObject gruntPrefab;
        [SerializeField] private GameObject speederPrefab;
        [SerializeField] private GameObject minibossPrefab;
        [SerializeField] private GameObject ninjaPrefab;


        [Header("Settings")]
        [SerializeField] private EnemySpawnGroups enemySpawnGroups;

        public IEnumerable<Enemy> Enemies => _enemyRefs.Select(go => go.GetComponent<Enemy>());

        private Waypoint _waypoint;
        [ReadOnly]
        [SerializeField]
        private List<GameObject> _enemyRefs;
        private bool _isInMinionMode;
        private Vector3 _startingPosition;
        private Dictionary<EnemyPoolName, string> enemyPoolNameMap = new Dictionary<EnemyPoolName, string>();
        private Dictionary<EnemyPoolName, GameObject> enemyPrefabMap = new Dictionary<EnemyPoolName, GameObject>();
        
        private void Awake()
        {
            _waypoint = GetComponent<Waypoint>();
            _enemyRefs = new List<GameObject>();
            _isInMinionMode = true;
            _startingPosition = _waypoint.GetWaypointPosition(0);

            // map enemy pool name to enemy prefab
            enemyPoolNameMap[EnemyPoolName.Grunt] = GRUNT_POOL_NAME;
            enemyPrefabMap[EnemyPoolName.Grunt] = gruntPrefab;
            enemyPoolNameMap[EnemyPoolName.Speeder] = SPEEDER_POOL_NAME;
            enemyPrefabMap[EnemyPoolName.Speeder] = speederPrefab;
            enemyPoolNameMap[EnemyPoolName.MiniBoss] = MINIBOSS_POOL_NAME;
            enemyPrefabMap[EnemyPoolName.MiniBoss] = minibossPrefab;
            enemyPoolNameMap[EnemyPoolName.Ninja] = NINJA_POOL_NAME;
            enemyPrefabMap[EnemyPoolName.Ninja] = ninjaPrefab;

            // Create pools
            ObjectPooler.Instance.CreatePool(GRUNT_POOL_NAME, gruntPrefab, 50, new ObjectPoolOptions(_startingPosition));
            ObjectPooler.Instance.CreatePool(SPEEDER_POOL_NAME, speederPrefab, 20, new ObjectPoolOptions(_startingPosition));
            ObjectPooler.Instance.CreatePool(MINIBOSS_POOL_NAME, minibossPrefab, 10, new ObjectPoolOptions(_startingPosition));
            ObjectPooler.Instance.CreatePool(NINJA_POOL_NAME, ninjaPrefab, 1, new ObjectPoolOptions(_startingPosition));
        }

        private void OnEnable()
        {
            BattleManager.OnMinionWaveUpdate += MinionWaveTimeUpdate;
            BattleManager.OnMinionWaveOver += MinionWaveOver;
            BattleManager.OnBossWaveUpdate += BossWaveTimeUpdate;
            Enemy.OnDeath += DespawnEnemy;
            Enemy.OnEndReached += DespawnEnemy;
        }

        private void OnDisable()
        {
            BattleManager.OnMinionWaveUpdate -= MinionWaveTimeUpdate;
            BattleManager.OnMinionWaveOver -= MinionWaveOver;
            BattleManager.OnBossWaveUpdate -= BossWaveTimeUpdate;
            Enemy.OnDeath -= DespawnEnemy;
            Enemy.OnEndReached -= DespawnEnemy;
        }

        private void MinionWaveTimeUpdate(float waveTimer, float waveTimerRemaining)
        {
            foreach (SpawnGroup spawnGroup in enemySpawnGroups.SpawnGroups)
            {
                if (spawnGroup.coroutine == null)
                {
                    spawnGroup.coroutine = StartCoroutine(SpawnEnemyGroup(spawnGroup));
                }
            }
        }

        private void MinionWaveOver()
        {
            // add total alive enemies health to the health of the boss to spawn
            int totalMinionHealth = _enemyRefs.Select(e => e.GetComponent<Enemy>().EnemyHealth.CurrentHealth)?.Sum() ?? 0;
            _enemyRefs.ForEach(go => DespawnEnemy(go.GetComponent<Enemy>()));
            RemoveAllEnemyRef();

            // spawn ninja boss
            GameObject newInstance = ObjectPooler.Instance.GetInstanceFromPool(NINJA_POOL_NAME);
            Enemy component = newInstance.GetComponent<Enemy>();
            component.SpawnInit();
            component.EnemyHealth.IncreaseCurrentHealth(totalMinionHealth);
            AddEnemyRef(newInstance);
        }

        private void BossWaveTimeUpdate()
        {

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

            yield return new WaitForSeconds(spawnGroup.groupDelay);
            spawnGroup.coroutine = null;
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

        private void RemoveAllEnemyRef()
        {
            _enemyRefs.Clear();
            OnEnemiesChanged?.Invoke(new List<Enemy>());
        }
    }
}