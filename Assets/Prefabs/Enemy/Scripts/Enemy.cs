using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Action<Enemy> OnEndReached;
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private int deathCoinReward;
    [SerializeField] private Waypoint waypoint;

    private float MoveSpeed => moveSpeed;
    private Waypoint Waypoint => waypoint;
    private Vector3 CurrentPointPosition => waypoint.GetWaypointPosition(_currentWaypointIndex);
    private SpriteRenderer _spriteRenderer;
    private Vector3 _lastPointPosition;
    private int _currentWaypointIndex;
    private EnemyHealth _enemyHealth;
    
    private void Start()
    {
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
        transform.position = Vector3.MoveTowards(transform.position, CurrentPointPosition, MoveSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        _spriteRenderer.flipX = CurrentPointPosition.x <= _lastPointPosition.x;
    }

    private bool CurrentPointPositionReached()
    {
        var distanceToNextPointPosition = (transform.position - CurrentPointPosition).magnitude;
        if (!(distanceToNextPointPosition < 0.1f)) return false;
        _lastPointPosition = transform.position;
        return true;
    }

    private void UpdateCurrentPointIndex()
    {
        int lastWaypointIndex = Waypoint.Points.Length - 1;
        if (_currentWaypointIndex < lastWaypointIndex)
        {
            _currentWaypointIndex++;
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

    public void SetWaypoint(Waypoint wp)
    {
        waypoint = wp;
    }
}
