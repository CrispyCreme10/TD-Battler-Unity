using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class Projectile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D rigidBody;

        [Header("Attributes")]
        [SerializeField] private float speed = 11f;

        [Header("Display")]
        [ReadOnly]
        [SerializeField]
        private GameObject _target;

        public EnemyDebuff EnemyDebuff { get => _enemyDebuff; set => _enemyDebuff = value; }

        public string SourceObjName => _sourceObjName;

        private string _sourceObjName;
        private Damageable _damageable;
        private EnemyDebuff _enemyDebuff;

        private void Awake()
        {
            _damageable = GetComponent<Damageable>();
        }

        private void OnEnable()
        {
            Enemy.OnDeath += SetEnemyTarget;
            Enemy.OnEndReached += SetEnemyTarget;
            _damageable.AfterDamage += Damageable_AfterDamage;
        }

        private void OnDisable()
        {
            Enemy.OnDeath -= SetEnemyTarget;
            Enemy.OnEndReached -= SetEnemyTarget;
            _damageable.AfterDamage -= Damageable_AfterDamage;
        }

        public void SetSourceObjectName(string name)
        {
            _sourceObjName = name;
        }

        private void SetEnemyTarget(Enemy enemy)
        {
            SetTarget(enemy.gameObject);
        }

        public void SetTarget(GameObject target)
        {
            if (this._target == target)
            {
                this._target = null;
                return;
            }

            this._target = target;
        }

        public void SetDamage(float _damage)
        {
            _damageable.SetDamage(_damage);
        }

        private void FixedUpdate()
        {
            if (!_target || !_target.activeSelf)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 targetPos = _target.transform.position;

            Vector2 direction = (targetPos - transform.position).normalized;

            float angle = Mathf.Atan2(targetPos.y - transform.position.y, targetPos.x - transform.position.x) * Mathf.Rad2Deg - 90f;
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