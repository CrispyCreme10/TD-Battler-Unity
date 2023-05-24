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
        [SerializeField] private HealthUpgrades_SO healthUpgrades;

        public EnemyHealth EnemyHealth => _enemyHealth;
        public int DeathCoinReward => deathCoinReward;

        private float MoveSpeed => moveSpeed;
        private Waypoint _waypoint;
        private Vector3 _currentPointPosition;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _lastPointPosition;
        private int _currentWaypointIndex;
        private EnemyHealth _enemyHealth;

        private void Awake() 
        {
            _waypoint = GameObject.Find("EnemySpawner").GetComponent<Waypoint>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _enemyHealth = GetComponent<EnemyHealth>();
            Init();
        }

        private void OnEnable()
        {
            EnemySpawner.OnTimerChanged += OnTimerChanged;
        }

        private void OnDisable()
        {
            EnemySpawner.OnTimerChanged -= OnTimerChanged;
        }

        private void Start()
        {
            
        }

        private void Update()
        {

            Move();

            if (CurrentPointPositionReached())
            {
                UpdateCurrentPointIndex();
            }
        }

        private void Init()
        {
            _currentWaypointIndex = 0;
            _currentPointPosition = _waypoint.GetWaypointPosition(_currentWaypointIndex);
            _lastPointPosition = transform.position;
        }

        private void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPointPosition, MoveSpeed * Time.deltaTime);
        }

        private bool CurrentPointPositionReached()
        {
            var distanceToNextPointPosition = (transform.position - _currentPointPosition).magnitude;
            if (!(distanceToNextPointPosition < 0.1f)) return false;
            _lastPointPosition = transform.position;
            return true;
        }

        private void UpdateCurrentPointIndex()
        {
            int lastWaypointIndex = _waypoint.Points.Length - 1;
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

        private void EndPointReached()
        {
            OnEndReached?.Invoke(this);
        }

        public void Death()
        {
            OnDeath?.Invoke(this);
        }

        public void ResetEnemy()
        {
            _enemyHealth.RefreshHealth();
            Init();
        }

        private void OnTimerChanged(float startingTime, float remainingTime)
        {
            int newHealth = healthUpgrades.GetCurrentHealth(startingTime, remainingTime, LevelManager.Instance.CurrentWave);
            if (newHealth > 0)
            {
                try
                {
                    if (_enemyHealth == null) _enemyHealth = GetComponent<EnemyHealth>();
                    _enemyHealth.UpdateInitialHealth(newHealth);
                }
                catch (System.Exception e)
                {
                    Debug.Log(name);
                    throw e;
                }
            }
        }

        public void SetHealth(float startingTime, float remainingTime)
        {
            int newHealth = healthUpgrades.GetCurrentHealth(startingTime, remainingTime, LevelManager.Instance.CurrentWave);
            Debug.Log($"{this.name} {newHealth}");
            try
            {
                if (_enemyHealth == null) _enemyHealth = GetComponent<EnemyHealth>();
                _enemyHealth.UpdateInitialHealth(newHealth);
                _enemyHealth.RefreshHealth();
            }
            catch (System.Exception e)
            {
                Debug.Log(name);
                throw e;
            }
        }

        private void OnTimerEnd()
        {

        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            
        }

        public Vector3 GetTargetPosition()
        {
            return transform.position;
        }
    }

    public class DistanceCovered
    {
        public float TotalDistance { get; private set; }

        public void UpdateDistance(float totalDistanceFromPrevWaypoints, float distanceFromLastWaypoint)
        {
            TotalDistance = totalDistanceFromPrevWaypoints + distanceFromLastWaypoint;
        }
    }
}