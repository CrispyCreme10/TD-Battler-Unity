using UnityEngine;

namespace TDBattler.Runtime
{
    public class TowerProjectile : MonoBehaviour
    {
        [SerializeField] protected Transform projectileSpawnPosition;
        [SerializeField] protected float delayBtwAttacks = 2f;
        [SerializeField] protected float damage = 2f;

        public float Damage { get; set; }
        public float DelayPerShot { get; set; }
        protected float _nextAttackTime;
        protected Tower _tower;
        protected Projectile _currentProjectileLoaded;

        private void Start() {
            _tower = GetComponent<Tower>();

            Damage = damage;
            DelayPerShot = delayBtwAttacks;
            LoadProjectile();
        }

        private void Update() {
            if (true)
            {
                LoadProjectile();
            }
            if (Time.time > _nextAttackTime)
            {

            }
        }

        protected virtual void LoadProjectile()
        {

        }
    }
}