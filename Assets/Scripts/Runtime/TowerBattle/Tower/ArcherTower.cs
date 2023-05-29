using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class ArcherTower : Tower
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
    }
}
