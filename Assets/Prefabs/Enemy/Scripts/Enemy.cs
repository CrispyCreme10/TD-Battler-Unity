using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Action<Enemy> OnEndReached;
    public static Action<Enemy> OnDeath;
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private int deathCoinReward;

    private float MoveSpeed => moveSpeed;
    public EnemyHealth EnemyHealth => _enemyHealth;
    private Waypoint _waypoint;
    private Vector3 _currentPointPosition;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _lastPointPosition;
    private int _currentWaypointIndex;
    private EnemyHealth _enemyHealth;
    
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
        _enemyHealth.ResetHealth();
        Init();
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }
}
