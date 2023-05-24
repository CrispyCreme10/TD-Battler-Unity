using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private List<HealthUpgrades_SO> enemyHealthUpgrades;

        private void OnEnable()
        {
            BattleManager.OnMinionWaveUpdate += EnemyTimerChanged;
        }

        private void OnDisable()
        {
            BattleManager.OnMinionWaveUpdate -= EnemyTimerChanged;
        }

        private void EnemyTimerChanged(float initTime, float timeRemaining)
        {
            enemyHealthUpgrades.ForEach(eHealth => eHealth.SetCurrentHealth(initTime, timeRemaining, LevelManager.Instance.CurrentWave));
        }
    }
}
