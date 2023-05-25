using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class FreezerTower : Tower
    {
        public override bool GenerateEnemyDebuff(out EnemyDebuff enemyDebuff)
        {
            enemyDebuff = new EnemyDebuff(GetStat(StatType.SlowTarget), EnemyDebuffType.MovementSpeed);
            return true;
        }
    }
}
