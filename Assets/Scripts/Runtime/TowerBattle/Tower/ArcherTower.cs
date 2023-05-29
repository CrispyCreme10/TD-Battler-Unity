using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class ArcherTower : Tower
    {
        protected override void GetCurrentEnemyTarget()
        {
            base.GetCurrentEnemyTarget();

            _currentEnemyTarget = GetFirstEnemy();
        }
    }
}
