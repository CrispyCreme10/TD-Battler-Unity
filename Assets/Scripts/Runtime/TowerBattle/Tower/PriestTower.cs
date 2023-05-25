using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class PriestTower : Tower
    {
        public override void OnMerge(TowerBattleManager towerBattleManager)
        {
            int manaIncreaseAmount = (int)GetStat(StatType.GenerateMana);
            towerBattleManager.TryIncreaseMana(manaIncreaseAmount);
        }
    }
}
