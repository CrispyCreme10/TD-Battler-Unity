using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class PriestTower : Tower
    {
        protected override void GetCurrentEnemyTarget()
        {
            base.GetCurrentEnemyTarget();

            _currentEnemyTarget = GetFirstEnemy();
        }

        public override void OnMerge(TowerBattleManager towerBattleManager)
        {
            int manaIncreaseAmount = (int)GetStat(StatType.GenerateMana);
            towerBattleManager.TryIncreaseMana(manaIncreaseAmount);
        }
    }
}
