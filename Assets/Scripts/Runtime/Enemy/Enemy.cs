using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class Enemy : MonoBehaviour
    {
        public static Action<Enemy> OnEndReached;
        public static Action<Enemy> OnDeath;

        [Header("Attributes")]
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private int deathCoinReward = 10;


        public EnemyHealth EnemyHealth => _enemyHealth;
        public int DeathCoinReward => deathCoinReward;
        public float DistanceTraveled => _distanceTraveled;

        private float MoveSpeed => moveSpeed;
        private Waypoint _waypoint;
        private Vector3 _currentPointPosition;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _lastPointPosition;
        private int _currentWaypointIndex;
        private EnemyHealth _enemyHealth;
        private float _distanceTraveled;
        private List<Projectile> _projectilesTargeting = new List<Projectile>();
        private float _timeAlive;

        private void Awake()
        {
            _waypoint = GameObject.Find("EnemySpawner").GetComponent<Waypoint>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _enemyHealth = GetComponent<EnemyHealth>();
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        private void Start()
        {

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

            UpdateDistanceTraveled();
        }

        public void SpawnInit()
        {
            _currentWaypointIndex = 0;
            _currentPointPosition = _waypoint.GetWaypointPosition(_currentWaypointIndex);
            _lastPointPosition = transform.position;
            _distanceTraveled = 0;
            _timeAlive = 0;
            _enemyHealth.SpawnInit();
        }

        private void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPointPosition, MoveSpeed * Time.deltaTime);
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
            _distanceTraveled = _timeAlive * moveSpeed;
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
    }
}