using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class FreezerTower : Tower
    {
        [Header("Freezer Attributes")]
        [ReadOnly]
        [SerializeField]
        private const float MAX_SLOW_PERCENT = 50;
        private float MaxSlowPercent => MAX_SLOW_PERCENT * 0.01f;

        public override bool GenerateEnemyDebuff(out EnemyDebuff enemyDebuff)
        {
            enemyDebuff = null;
            if (_currentEnemyTarget.FreezerSlowPercent >= MaxSlowPercent) return false;
            
            enemyDebuff = new EnemyDebuff(GetStat(StatType.SlowTarget), TowerModType.Freezer);
            return true;
        }

        protected override void SetCurrentEnemyTarget()
        {
            if (base.NoEnemyTarget())
            {
                _currentEnemyTarget = null;
                return;
            }

            _currentEnemyTarget = GetFirstNonMaxSlowedEnemy();
        }

        private Enemy GetFirstNonMaxSlowedEnemy()
        {
            IOrderedEnumerable<Enemy> enemiesOrderedByDistance = _enemiesInRange.OrderByDescending(e => e.DistanceTraveled);
            _enemiesDistance = enemiesOrderedByDistance.Select(e => new EnemyDistance{ Distance = e.DistanceTraveled, Name = e.name})
                .ToList();
            return enemiesOrderedByDistance.OrderBy(e => e.FreezerSlowPercent)
                .FirstOrDefault();
        }
    }

    // need to track the total slow percentage on each enemy that has been applied by ANY freezer tower
    // so we can know when the max slow percentage has been reached. this will continously be checked
    // by all freezer tower instances so they know how to 1. choose their current target 2. construct
    // the freezer debuff data that gets sent to the enemy

}
