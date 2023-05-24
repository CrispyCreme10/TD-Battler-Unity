using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class Projectile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D rigidBody;

        [Header("Attributes")]
        [SerializeField] private float speed = 5f;

        [Header("Display")]
        [ReadOnly]
        [SerializeField]
        private Enemy _enemyTarget;

        // Scripts
        private Damageable _damageable;

        private void Awake()
        {
            _damageable = GetComponent<Damageable>();
        }

        private void OnEnable()
        {
            Enemy.OnDeath += SetTarget;
            Enemy.OnEndReached += SetTarget;
            _damageable.AfterDamage += Damageable_AfterDamage;
        }

        private void OnDisable()
        {
            Enemy.OnDeath -= SetTarget;
            Enemy.OnEndReached -= SetTarget;
            _damageable.AfterDamage -= Damageable_AfterDamage;
        }

        public void SetTarget(Enemy _enemy)
        {
            if (this._enemyTarget == _enemy)
            {
                this._enemyTarget = null;
                return;
            }

            this._enemyTarget = _enemy;
        }

        public void SetDamage(float _damage)
        {
            _damageable.SetDamage(_damage);
        }

        private void FixedUpdate()
        {
            if (!_enemyTarget || !_enemyTarget.isActiveAndEnabled)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 enemyTargetPos = _enemyTarget.GetTargetPosition();

            Vector2 direction = (enemyTargetPos - transform.position).normalized;

            float angle = Mathf.Atan2(enemyTargetPos.y - transform.position.y, enemyTargetPos.x - transform.position.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            transform.rotation = targetRotation;

            rigidBody.velocity = direction * speed;
        }

        private void Damageable_AfterDamage()
        {
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null && enemy == _enemyTarget)
            {
                enemy.EnemyHealth.DealDamage(_damageable.Damage);
                _damageable.PerformedDamage();
            }
        }
    }
}