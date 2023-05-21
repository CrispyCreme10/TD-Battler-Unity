using UnityEngine;

namespace TDBattler.Runtime
{
    public class Projectile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D rigidBody;

        [Header("Attributes")]
        [SerializeField] private float speed = 5f;

        private Enemy _enemy;

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
            if (this._enemy == _enemy)
            {
                this._enemy = null;
                return;
            }

            this._enemy = _enemy;
        }

        public void SetDamage(float _damage)
        {
            _damageable.SetDamage(_damage);
        }

        private void FixedUpdate()
        {
            if (!_enemy)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 enemyTargetPos = _enemy.GetTargetPosition();

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
    }
}