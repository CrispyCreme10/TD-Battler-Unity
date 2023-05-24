using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class EnemyHealth : MonoBehaviour
    {
        public static Action<Enemy> OnEnemyKilled;
        public static Action<Enemy> OnEnemyHit;

        [Header("References")]
        [SerializeField] private HealthUpgrades_SO healthUpgrades;
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private Transform barPosition;

        [Header("Attributes")]
        [SerializeField] private int maxHealth = 100_000;
        [SerializeField] private int currentHealth;

        public int CurrentHealth => currentHealth;
        private string _healthBar;
        private Enemy _enemy;
        private EnemyHealthContainer _container;
        private List<GameObject> _damageTextObjs = new List<GameObject>();

        private void Awake()
        {
            CreateHealthBar();
            _enemy = GetComponent<Enemy>();
        }

        private void Start()
        {
            
        }

        private void CreateHealthBar()
        {
            GameObject newBar = Instantiate(healthBarPrefab, barPosition.position, Quaternion.identity, transform);
            _container = newBar.GetComponent<EnemyHealthContainer>();
        }

        public void DealDamage(int damageRecieved)
        {
            if (damageTextPrefab != null)
            {
                ShowDamageText(damageRecieved);
            }

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

        private void ShowDamageText(int damage)
        {
            var go = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, transform);
            go.GetComponent<TextMesh>().text = damage.ToString();
            _damageTextObjs.Add(go);
        }

        public void SpawnInit()
        {
            currentHealth = healthUpgrades.SpawnHealth;
            UpdateHealthText();
            _damageTextObjs.ForEach(obj => Destroy(obj));
            _damageTextObjs.Clear();
        }

        private void UpdateHealthText()
        {
            _container.SetHealthText(currentHealth);
        }
    }
}