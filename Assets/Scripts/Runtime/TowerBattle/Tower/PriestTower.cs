using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class PriestTower : Tower
    {
        protected override void SetCurrentEnemyTarget()
        {
            if (base.NoEnemyTarget())
            {
                _currentEnemyTarget = null;
                return;
            }

            _currentEnemyTarget = GetFirstEnemy();
        }

        public override void OnMerge(TowerBattleManager towerBattleManager)
        {
            int manaIncreaseAmount = (int)GetStat(StatType.GenerateMana);
            towerBattleManager.TryIncreaseMana(manaIncreaseAmount);
        }
    }
}
