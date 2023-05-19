using System;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class EnemyHealth : MonoBehaviour
    {
        public static Action<Enemy> OnEnemyKilled;
        public static Action<Enemy> OnEnemyHit;

        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private Transform barPosition;
        [SerializeField] private int initialHealth = 250;
        [SerializeField] private int maxHealth = 100_000;
        [SerializeField] private int currentHealth;

        public int CurrentHealth => currentHealth;
        private string _healthBar;
        private Enemy _enemy;
        private EnemyHealthContainer _container;

        private void Start()
        {
            CreateHealthBar();
            Init();

            _enemy = GetComponent<Enemy>();
        }

        private void CreateHealthBar()
        {
            GameObject newBar = Instantiate(healthBarPrefab, barPosition.position, Quaternion.identity, transform);
            _container = newBar.GetComponent<EnemyHealthContainer>();
        }

        public void DealDamage(int damageRecieved)
        {
            currentHealth -= damageRecieved;
            if (CurrentHealth <= 0)
            {
                currentHealth = 0;
                _enemy.Death();
                OnEnemyKilled?.Invoke(_enemy);
                return;
            }

            UpdateHealthText();
            OnEnemyHit?.Invoke(_enemy);
        }

        public void Init()
        {
            currentHealth = initialHealth;
            UpdateHealthText();
        }

        private void UpdateHealthText()
        {
            _container.SetHealthText(currentHealth);
        }

        public void UpdateInitialHealth(int newHealth)
        {
            initialHealth = newHealth;
        }
    }
}