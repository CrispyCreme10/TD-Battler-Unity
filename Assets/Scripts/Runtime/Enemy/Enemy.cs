using System;
using System.Collections;
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

            Move();

            var distanceToNextWaypoint = (transform.position - _currentPointPosition).magnitude;
            if (CurrentPointPositionReached(distanceToNextWaypoint))
            {
                UpdateCurrentPointIndex();
            }

            UpdateDistanceTraveled(distanceToNextWaypoint);
        }

        public void SpawnInit()
        {
            _currentWaypointIndex = 0;
            _currentPointPosition = _waypoint.GetWaypointPosition(_currentWaypointIndex);
            _lastPointPosition = transform.position;
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

        private void UpdateDistanceTraveled(float distanceToNextPointPosition)
        {
            _distanceTraveled = Mathf.Abs(_waypoint.GetDistanceBetweenPoints(_currentWaypointIndex - 1)) + 
                (Mathf.Abs(_waypoint.GetDistanceBetweenPoints(_currentWaypointIndex)) - Mathf.Abs(distanceToNextPointPosition));
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
    }
}