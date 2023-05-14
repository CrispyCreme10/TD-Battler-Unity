using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotationPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firingPoint;

    [Header("Attributes")]
    [SerializeField] private float attackRange;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float pbs = 1f; // projectiles per second

    private Enemy _currentEnemyTarget;
    private List<Enemy> _enemiesInRange;
    private float timeUntilFire;

    private void Start()
    {
        _enemiesInRange = new List<Enemy>();
    }

    private void Update()
    {
        GetCurrentEnemyTarget();
        RotateTowardsTarget();

        if (_currentEnemyTarget != null) 
        {
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / pbs)
            {
                Shoot();
                timeUntilFire = 0f;
            }
        }
    }

    private void Shoot()
    {
        GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, Quaternion.identity);
        Projectile projectileScript = projectileObj.GetComponent<Projectile>();
        projectileScript.SetTarget(_currentEnemyTarget.transform);
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
        if (_currentEnemyTarget == null)
        {
            return;
        }

        float angle = Mathf.Atan2(_currentEnemyTarget.transform.position.y - transform.position.y, _currentEnemyTarget.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        rotationPoint.rotation = targetRotation;
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
