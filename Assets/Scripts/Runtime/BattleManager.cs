using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class BattleManager : MonoBehaviour
    {
        public static Action<float, float> OnMinionWaveUpdate;
        public static Action OnMinionWaveOver;
        public static Action OnBossWaveUpdate;
        public static Action OnGameOver;

        [Header("References")]
        [SerializeField] private EnemySpawner enemySpawner;

        [Header("Settings")]
        [SerializeField] private int minionWaveTimer = 120;
        [SerializeField] private float debugBattleDelay = 3f;

        [Header("Display Timers")]
        [ReadOnly]
        [SerializeField]
        private float battleTimeElapsed;
        [ReadOnly]
        [SerializeField]
        private float waveTimeRemaining;

        private Coroutine _initBattleCoroutine;
        private bool _isGameOver;
        private bool _isBattleInitialized;
        private bool _isMinionWaveActive;
        private bool _isBossWaveActive;

        private void Awake()
        {
            battleTimeElapsed = 0;
        }

        private void OnEnable()
        {
            LevelManager.OnAllLivesLost += GameOver;
        }

        private void OnDisable()
        {
            LevelManager.OnAllLivesLost -= GameOver;
        }

        private void Update()
        {
            if (_isGameOver) return;

            battleTimeElapsed += Time.deltaTime;

            if (_initBattleCoroutine == null) _initBattleCoroutine = StartCoroutine(InitMinionWave());
            if (!_isMinionWaveActive) return;

            // minion waves
            if (waveTimeRemaining <= 0)
            {
                // end minion wave
                _isMinionWaveActive = false;
                // setup boss wave vars
                _isBossWaveActive = true;
                Debug.Log("BOSS TIME");

                OnMinionWaveOver?.Invoke();
            }

            if (_isMinionWaveActive)
            {
                waveTimeRemaining -= Time.deltaTime;
                OnMinionWaveUpdate?.Invoke(minionWaveTimer, waveTimeRemaining);
                return;
            }

            // boss waves
            if (!_isBossWaveActive) return;
            
            if (_isBossWaveActive)
            {
                OnBossWaveUpdate?.Invoke();
            }
        }

        private IEnumerator InitMinionWave()
        {
            yield return new WaitForSeconds(debugBattleDelay);

            // wait for animations

            ResetMinionWaveTimer();
            _isMinionWaveActive = true;
        }

        private IEnumerator InitBossWave()
        {
            yield return new WaitForSeconds(debugBattleDelay);

            // wait for animations

            _isBossWaveActive = true;
        }

        private void ResetMinionWaveTimer()
        {
            waveTimeRemaining = minionWaveTimer;
        }

        private void GameOver()
        {
            _isGameOver = true;
            Time.timeScale = 0;
            OnGameOver?.Invoke();
        }
    }
}
