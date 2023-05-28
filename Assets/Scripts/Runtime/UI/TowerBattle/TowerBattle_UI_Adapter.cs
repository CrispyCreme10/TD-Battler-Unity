using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDBattler.Runtime
{
    // responsible for connecting TowerBattle_UI document to logic
    public class TowerBattle_UI_Adapter : MonoBehaviour
    {
        public static Action<string> OnEnergyIncrease;

        [Header("References")]
        [SerializeField] private TowerBattle_UI document;
        [SerializeField] private TowerSpawner towerSpawner;
        [SerializeField] private TowerBattleManager towerBattleManager;
        [SerializeField] private PlayerManager playerManager;

        private void OnEnable()
        {
            TowerBattleManager.OnManaChange += OnManaChange;
            BattleManager.OnMinionWaveUpdate += SetRoundTimer;
            EnemySpawner.OnEnemiesChanged += OnEnemiesChange;
        }

        private void OnDisable()
        {
            TowerBattleManager.OnManaChange -= OnManaChange;
            BattleManager.OnMinionWaveUpdate -= SetRoundTimer;
            EnemySpawner.OnEnemiesChanged -= OnEnemiesChange;
        }

        public void SpawnTower()
        {
            towerBattleManager.SpawnTower();
        }

        public int GetMana()
        {
            return towerBattleManager.Mana;
        }

        public int GetTowerCost()
        {
            return towerBattleManager.TowerCost;
        }

        public IEnumerable<string> GetTowerNames()
        {
            return playerManager.SelectedTowers.Towers.Select(tower => tower.GetComponent<Tower>().TowerData.name);
        }

        public void IncreaseEnergyLevel(string towerName)
        {
            OnEnergyIncrease?.Invoke(towerName);
        }

        #region Incoming Events
        private void OnManaChange(int mana, int towerCost, List<int> energyCosts, bool fieldIsFull)
        {
            document.OnManaChange(mana, towerCost, energyCosts, fieldIsFull);
        }
        
        private void OnLivesChange(int totalLives)
        {
            document.OnLivesChange(LevelManager.Instance.TotalLives);
        }

        private void OnEnemiesChange(IEnumerable<Enemy> enemies)
        {
            document.OnEnemiesChange(enemies.Count());
        }

        private void SetRoundTimer(float startingTime, float timeInSeconds)
        {
            string formattedText = "Boss";
            if (timeInSeconds > 0)
            {
                int seconds = Mathf.FloorToInt(timeInSeconds % 60);
                formattedText = $"0{Mathf.FloorToInt(timeInSeconds / 60)}:{(seconds < 10 ? '0' + seconds.ToString() : seconds)}";
            }
            document.SetRoundTimer(formattedText);
        }

        #endregion
    }
}