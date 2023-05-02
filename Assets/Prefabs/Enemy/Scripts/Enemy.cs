using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Action<Enemy> OnEndReached;
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private int deathCoinReward;

    private float MoveSpeed => moveSpeed;
    private Waypoint _waypoint;
    private Vector3 _currentPointPosition;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _lastPointPosition;
    private int _currentWaypointIndex;
    private EnemyHealth _enemyHealth;
    
    private void Start()
    {
        _waypoint = GameObject.Find("Spawner").GetComponent<Waypoint>();
        _currentPointPosition = _waypoint.GetWaypointPosition(0);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lastPointPosition = transform.position;
        _currentWaypointIndex = 0;
        _enemyHealth = GetComponent<EnemyHealth>();
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
        ResetEnemy();
    }
    
    private void ResetEnemy()
    {
        _enemyHealth.ResetHealth();
        ObjectPooler.ReturnToPool(gameObject);
    }
}
