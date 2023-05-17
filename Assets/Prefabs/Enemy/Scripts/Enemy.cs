using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Action<Enemy> OnEndReached;
    public static Action<Enemy> OnDeath;

    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private int deathCoinReward = 10;
    [SerializeField] private HealthUpgrades_SO healthUpgrades;

    public EnemyHealth EnemyHealth => _enemyHealth;
    public int DeathCoinReward => deathCoinReward;

    private float MoveSpeed => moveSpeed;
    private Waypoint _waypoint;
    private Vector3 _currentPointPosition;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _lastPointPosition;
    private int _currentWaypointIndex;
    private EnemyHealth _enemyHealth;

    private void OnEnable()
    {
        Spawner.OnTimerChanged += OnTimerChanged;
    }

    private void OnDisable()
    {
        Spawner.OnTimerChanged -= OnTimerChanged;
    }

    private void Start()
    {
        _waypoint = GameObject.Find("Spawner").GetComponent<Waypoint>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemyHealth = GetComponent<EnemyHealth>();
        Init();
    }

    private void Update()
    {
        Move();
        Rotate();

        if (CurrentPointPositionReached())
        {
            UpdateCurrentPointIndex();
        }
    }

    private void Init()
    {
        _currentWaypointIndex = 0;
        _currentPointPosition = _waypoint.GetWaypointPosition(_currentWaypointIndex);
        _lastPointPosition = transform.position;
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _currentPointPosition, MoveSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        _spriteRenderer.flipX = _currentPointPosition.x <= _lastPointPosition.x;
    }

    private bool CurrentPointPositionReached()
    {
        var distanceToNextPointPosition = (transform.position - _currentPointPosition).magnitude;
        if (!(distanceToNextPointPosition < 0.1f)) return false;
        _lastPointPosition = transform.position;
        return true;
    }

    private void UpdateCurrentPointIndex()
    {
        int lastWaypointIndex = _waypoint.Points.Length - 1;
        if (_currentWaypointIndex < lastWaypointIndex)
        {
            _currentWaypointIndex++;
            _currentPointPosition = _waypoint.GetWaypointPosition(_currentWaypointIndex);
        }
        else
        {
            EndPointReached();
        }
    }

    private void EndPointReached()
    {
        OnEndReached?.Invoke(this);
        ResetEnemy();
    }

    public void Death()
    {
        OnDeath?.Invoke(this);
        ResetEnemy();
    }

    private void ResetEnemy()
    {
        _enemyHealth.Init();
        Init();
    }

    private void OnTimerChanged(float startingTime, float remainingTime)
    {
        int newHealth = healthUpgrades.GetCurrentHealth(startingTime, remainingTime, LevelManager.Instance.CurrentWave);
        if (newHealth > 0)
        {
            try
            {
                if (_enemyHealth == null) _enemyHealth = GetComponent<EnemyHealth>();
                _enemyHealth.UpdateInitialHealth(newHealth);
            }
            catch (System.Exception e)
            {
                Debug.Log(name);
                throw e;
            }
        }
    }

    private void OnTimerEnd()
    {

    }
}