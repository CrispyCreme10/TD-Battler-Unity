using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDBattler.Runtime
{
    // responsible for connecting TowerBattle_UI document to logic
    public class TowerBattle_UI_Adapter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TowerBattle_UI document;
        [SerializeField] private TowerSpawner towerSpawner;
        [SerializeField] private TowerBattleManager towerBattleManager;
        [SerializeField] private PlayerManager playerManager;

        private void OnEnable()
        {
            TowerBattleManager.OnManaChange += OnManaChange;
            EnemySpawner.OnTimerChanged += SetRoundTimer;
            TowerSpawner.OnTowerEnergyIncrease += OnTowerEnergyIncrease;
        }

        private void OnDisable()
        {
            TowerBattleManager.OnManaChange -= OnManaChange;
            EnemySpawner.OnTimerChanged -= SetRoundTimer;
            TowerSpawner.OnTowerEnergyIncrease -= OnTowerEnergyIncrease;
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
            return playerManager.SelectedTowers.Select(tower => tower.TowerData.name);
        }

        #region Events
        
        private void OnManaChange(int mana, int towerCost, List<int> energyCosts)
        {
            document.OnManaChange(mana, towerCost, energyCosts);
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

        private void OnTowerEnergyIncrease(int index, int energyLevel)
        {
            document.OnTowerEnergyIncrease(index, energyLevel);
        }

        #endregion
    }
}