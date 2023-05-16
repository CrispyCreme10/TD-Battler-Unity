using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Action<IEnumerable<Enemy>> OnEnemiesChanged;

    [Header("Settings")]
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private GameObject testGO;

    [Header("Fixed Delay")]
    [SerializeField] private float initSpawnDelay = 1f;
    [SerializeField] private float delayBtwSpawns = 1f;

    public IEnumerable<Enemy> Enemies => _enemyRefs.Select(go => go.GetComponent<Enemy>());

    private float _spawnTimer;
    private int _enemiesSpawned;
    private List<GameObject> _enemyRefs;

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
        _spawnTimer = initSpawnDelay;
        _enemyRefs = new List<GameObject>();
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer < 0)
        {
            _spawnTimer = delayBtwSpawns;
            if (_enemiesSpawned < enemyCount)
            {
                _enemiesSpawned++;
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject newInstance = ObjectPooler.Instance.GetInstanceFromPool();
        newInstance.SetActive(true);
        AddEnemyRef(newInstance);
    }

    private void DespawnEnemy(Enemy enemy)
    {
        RemoveEnemyRef(enemy);
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
