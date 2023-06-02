using System;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class LevelManager : MonoBehaviour
    {
        public static Action OnAllLivesLost;
        public static Action OnLivesLost;
        private static LevelManager _instance;
        
        [SerializeField] private int lives = 3;
        [SerializeField] private int totalLives = 3;
        [SerializeField] private int currentWave = 1;

        public static LevelManager Instance => _instance;
        public int TotalLives => totalLives;
        public int CurrentWave => currentWave;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        private void Start()
        {
            totalLives = lives;
            currentWave = 1;
        }

        private void ReduceLives(Enemy enemy)
        {
            totalLives--;
            OnLivesLost?.Invoke();
            if (TotalLives <= 0)
            {
                totalLives = 0;
                OnAllLivesLost?.Invoke();
            }
        }

        private void OnEnable()
        {
            Enemy.OnEndReached += ReduceLives;
        }

        private void OnDisable()
        {
            Enemy.OnEndReached -= ReduceLives;
        }
    }
}