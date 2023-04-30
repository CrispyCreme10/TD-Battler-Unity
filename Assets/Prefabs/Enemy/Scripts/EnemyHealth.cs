using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public static Action<Enemy> OnEnemyKilled;
    public static Action<Enemy> OnEnemyHit;

    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform barPosition;
    [SerializeField] private int initialHealth = 10;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    public int CurrentHealth => currentHealth;
    private string _healthBar;
    private Enemy _enemy;
    private EnemyHealthContainer _container;

    private void Start()
    {
        CreateHealthBar();
        currentHealth = initialHealth;

        _enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            DealDamage(5);
        }
    }

    private void CreateHealthBar()
    {
        GameObject newBar = Instantiate(healthBarPrefab, barPosition.position, Quaternion.identity, transform);
        _container = newBar.GetComponent<EnemyHealthContainer>();
        _container.SetHealthText(initialHealth);
    }

    public void DealDamage(int damageRecieved)
    {
        currentHealth -= damageRecieved;
        if (CurrentHealth <= 0)
        {
            currentHealth = 0;
            OnEnemyKilled?.Invoke(_enemy);
            return;
        }

        UpdateHealthText();
        OnEnemyHit?.Invoke(_enemy);
    }

    private void UpdateHealthText()
    {
        _container.SetHealthText(currentHealth);
    }

    public void ResetHealth()
    {
        currentHealth = initialHealth;
    }
}
