using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidBody;

    [Header("Attributes")]
    [SerializeField] private float speed = 5f;

    private Enemy enemy;
    private int damage;

    private void OnEnable()
    {
        Enemy.OnDeath += SetTarget;
        Enemy.OnEndReached += SetTarget;
    }

    private void OnDisable()
    {
        Enemy.OnDeath -= SetTarget;
        Enemy.OnEndReached += SetTarget;
    }

    public void SetTarget(Enemy _enemy)
    {
        if (enemy == _enemy) 
        {
            enemy = null;
            return;
        }

        enemy = _enemy;
    }

    public void SetDamage(int _damage)
    {
        damage = _damage;
    }

    private void FixedUpdate()
    {
        if (!enemy)
        {
            Destroy(gameObject);
            return;
        }
        Vector2 direction = (enemy.transform.position - transform.position).normalized;

        float angle = Mathf.Atan2(enemy.transform.position.y - transform.position.y, enemy.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        transform.rotation = targetRotation;

        rigidBody.velocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.EnemyHealth.DealDamage(damage);
        }
        Destroy(gameObject);
    }
}
