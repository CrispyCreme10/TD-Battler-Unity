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
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private float projectilesPerSecond = 1f; // projectiles per second

    private Enemy _currentEnemyTarget;
    private List<Enemy> _enemiesInRange;
    private float timeUntilFire;

    private void Start()
    {
        _enemiesInRange = new List<Enemy>();
        InitBoxCollider();
    }

    private void Update()
    {
        GetCurrentEnemyTarget();
        Quaternion? enemyDirection = RotateTowardsTarget();

        if (_currentEnemyTarget != null) 
        {
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / projectilesPerSecond)
            {
                Shoot(enemyDirection);
                timeUntilFire = 0f;
            }
        }
    }

    private void InitBoxCollider()
    {
        Waypoint waypoint = GameObject.Find("Spawner").GetComponent<Waypoint>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        var ((width, height), center) = waypoint.GetWaypointsBounds();
        Vector3 offset = center - transform.position;
        boxCollider.offset = new Vector2(offset.x / transform.localScale.x, offset.y / transform.localScale.y);
        boxCollider.size = new Vector2(width / transform.localScale.x, height / transform.localScale.y);
    }

    private void Shoot(Quaternion? enemyDir)
    {
        GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, enemyDir.HasValue ? enemyDir.Value : Quaternion.identity);
        Projectile projectileScript = projectileObj.GetComponent<Projectile>();
        projectileScript.SetTarget(_currentEnemyTarget);
        projectileScript.SetDamage(projectileDamage);
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

    private Quaternion? RotateTowardsTarget()
    {
        if (_currentEnemyTarget == null)
        {
            return null;
        }

        float angle = Mathf.Atan2(_currentEnemyTarget.transform.position.y - transform.position.y, _currentEnemyTarget.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        rotationPoint.rotation = targetRotation;
        return targetRotation;
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
