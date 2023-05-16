using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Action<IEnumerable<Enemy>> OnEnemiesChanged;
    public static Action<float> OnTimerStart;
    public static Action<float, float> OnTimerChanged;
    public static Action OnTimerEnd;

    [Header("Settings")]
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private bool timerMode = true;
    [SerializeField] private int timerInSeconds = 120;
    [SerializeField] private GameObject testGO;

    [Header("Fixed Delay")]
    [SerializeField] private float initSpawnDelay = 1f;
    [SerializeField] private float delayBtwSpawns = 1f;

    public IEnumerable<Enemy> Enemies => _enemyRefs.Select(go => go.GetComponent<Enemy>());

    private float _spawnTimer;
    private int _enemiesSpawned;
    private List<GameObject> _enemyRefs;
    private bool _isInMinionMode;
    private float _minionTimeRemaining;

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
        _isInMinionMode = true;
        _minionTimeRemaining = timerInSeconds;
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
