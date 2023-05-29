using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class Enemy : MonoBehaviour
    {
        public static Action<Enemy> OnEndReached;
        public static Action<Enemy> OnDeath;

        [Header("Attributes")]
        [SerializeField] private float initMoveSpeed = 1f;
        [SerializeField] private int deathCoinReward = 10;


        public EnemyHealth EnemyHealth => _enemyHealth;
        public int DeathCoinReward => deathCoinReward;
        public float DistanceTraveled => _distanceTraveled;
        public float ActiveMoveSpeed => _activeMoveSpeed;
        public float FreezerSlowPercent => _freezerSlowPercent;

        [Header("Tracking")]
        [ReadOnly]
        [SerializeField]
        private float _activeMoveSpeed;
        [ReadOnly]
        [SerializeField]
        private float _freezerSlowPercent;

        private Waypoint _waypoint;
        private Vector3 _currentPointPosition;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _lastPointPosition;
        private int _currentWaypointIndex;
        private EnemyHealth _enemyHealth;
        private float _distanceTraveled;
        private List<Projectile> _projectilesTargeting = new List<Projectile>();
        private float _timeAlive;
        private Vector3 _prevPosition;

        private void Awake()
        {
            _waypoint = GameObject.Find("EnemySpawner").GetComponent<Waypoint>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _enemyHealth = GetComponent<EnemyHealth>();
        }

        private void OnEnable()
        {
            _prevPosition = transform.position;
        }

        private void Update()
        {
            _timeAlive += Time.deltaTime;

            Move();

            var distanceToNextWaypoint = (transform.position - _currentPointPosition).magnitude;
            if (CurrentPointPositionReached(distanceToNextWaypoint))
            {
                UpdateCurrentPointIndex();
            }
        }

        private void FixedUpdate()
        {
            UpdateDistanceTraveled();
        }

        public void SpawnInit()
        {
            _currentWaypointIndex = 0;
            _currentPointPosition = _waypoint.GetWaypointPosition(_currentWaypointIndex);
            _lastPointPosition = transform.position;
            _distanceTraveled = 0;
            _timeAlive = 0;
            _activeMoveSpeed = initMoveSpeed;
            _enemyHealth.SpawnInit();
        }

        private void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPointPosition, _activeMoveSpeed * Time.deltaTime);
        }

        private bool CurrentPointPositionReached(float distanceToNextPointPosition)
        {
            if (!(distanceToNextPointPosition < 0.1f)) return false;
            _lastPointPosition = transform.position;
            return true;
        }

        private void UpdateCurrentPointIndex()
        {
            int lastWaypointIndex = _waypoint.Points.Count - 1;
            if (_currentWaypointIndex < lastWaypointIndex)
            {
                _currentWaypointIndex++;
                _currentPointPosition = _waypoint.GetWaypointPosition(_currentWaypointIndex);
            }
            else
            {
                EndPointReached();
            }
        }

        private void UpdateDistanceTraveled()
        {
            _distanceTraveled += Vector3.Distance(transform.position, _prevPosition);
            _prevPosition = transform.position;
        }

        private void EndPointReached()
        {
            OnEndReached?.Invoke(this);
        }

        public void Death()
        {
            OnDeath?.Invoke(this);

        }

        public Vector3 GetTargetPosition()
        {
            return transform.position;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var projectile = other.gameObject.GetComponent<Projectile>();
            if (projectile != null && _projectilesTargeting.Contains(projectile))
            {
                var damageable = projectile.gameObject.GetComponent<Damageable>();
                _enemyHealth.DealDamage(damageable.Damage);
                damageable.PerformedDamage();
                RemoveProjectile(projectile);
                if (projectile.EnemyDebuff != null)
                {
                    ApplyDebuff(projectile.EnemyDebuff);
                }
            }
        }

        public void AddProjectile(Projectile projectile)
        {
            _projectilesTargeting.Add(projectile);
        }

        public void RemoveProjectile(Projectile projectile)
        {
            _projectilesTargeting.Remove(projectile);
        }

        public void ApplyDebuff(EnemyDebuff enemyDebuff)
        {
            if (enemyDebuff.EnemyDebuffType == TowerModType.Freezer)
            {
                _freezerSlowPercent += enemyDebuff.ValueToApply;
                _activeMoveSpeed = initMoveSpeed * (1 - _freezerSlowPercent);
            }
        }
    }

    [Serializable]
    public class EnemyDebuff
    {
        [SerializeField] private float valueToApply;
        [SerializeField] private TowerModType sourceTowerType;

        public float ValueToApply => valueToApply;
        public TowerModType EnemyDebuffType => sourceTowerType;

        public EnemyDebuff(float value, TowerModType type)
        {
            valueToApply = value;
            sourceTowerType = type;
        }
    }

    public enum TowerModType
    {
        Freezer
    }
}