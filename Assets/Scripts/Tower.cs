using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float attackRange;

    private Enemy _currentEnemyTarget;
    private List<Enemy> _enemiesInRange;

    private void Start()
    {
        _enemiesInRange = new List<Enemy>();
    }

    private void Update()
    {
        GetCurrentEnemyTarget();
        RotateTowardsTarget();
    }

    private void GetCurrentEnemyTarget()
    {
        if (_enemiesInRange.Count <= 0)
        {
            _currentEnemyTarget = null;
            return;
        }

        _currentEnemyTarget = _enemiesInRange[0];
    }

    private void RotateTowardsTarget()
    {
        if (_currentEnemyTarget.Equals(null))
        {
            return;
        }

        Vector3 targetPos = _currentEnemyTarget.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.up, targetPos, transform.forward);
        transform.Rotate(0f, 0f, angle);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy newEnemy = other.GetComponent<Enemy>();
            _enemiesInRange.Add(newEnemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Remove(enemy);
            }
        }
    }
}
