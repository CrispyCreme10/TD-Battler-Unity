using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class BuckshotTower : Tower
    {
        [ReadOnly]
        [SerializeField]
        private float _firstShotDamage;
        private bool _isFirstShot;

        protected override void GetCurrentEnemyTarget()
        {
            base.GetCurrentEnemyTarget();

            if (_currentEnemyTarget == null)
            {
                _currentEnemyTarget = GetRandomEnemy();
                _isFirstShot = true;
                return;
            }
        }

        protected override void RefreshStats()
        {
            foreach (Stat stat in towerData.Stats)
            {
                if (stat.Type == StatType.Damage)
                {
                    _firstShotDamage = stat.GetValueModified(mergeLevel, energyLevel, towerData.PermanentLevel);
                    statMap[stat.Type] = stat.GetValue(mergeLevel, energyLevel, towerData.PermanentLevel);
                    continue;
                }

                statMap[stat.Type] = stat.GetValueModified(mergeLevel, energyLevel, towerData.PermanentLevel);
            }
        }

        protected override IEnumerator Attack()
        {
            var attackInterval = GetStat(StatType.AttackInterval);
            yield return new WaitForSeconds(attackInterval);

            anim.speed = 1 / attackInterval;

            GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, enemyDir.HasValue ? enemyDir.Value : Quaternion.identity, _projectileContainer.transform);
            Projectile projectileScript = projectileObj.GetComponent<Projectile>();
            projectileScript.SetSourceTower(towerData.Name);
            projectileScript.SetTarget(_currentEnemyTarget);
            float damage = _isFirstShot ? _firstShotDamage : GetStat(StatType.Damage);
            projectileScript.SetDamage(damage);
            projectileObj.GetComponent<SpriteRenderer>().color = towerData.DebugColor;
            projectileObj.name = $"{name} - {projectileObj.name}";

            if (GenerateEnemyDebuff(out EnemyDebuff enemyDebuff))
            {
                projectileScript.EnemyDebuff = enemyDebuff;
            }

            if (_currentEnemyTarget != null)
            {
                _currentEnemyTarget.AddProjectile(projectileScript);
            }

            _isFirstShot = false;
            currentCoroutine = null;
        }
    }
}
