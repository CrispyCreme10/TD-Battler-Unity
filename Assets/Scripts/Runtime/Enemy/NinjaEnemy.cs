using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class NinjaEnemy : Enemy
    {
        [Header("References")]
        [SerializeField] private TowerBattleManager towerBattleManager;

        [SerializeField] private int shurikenTargetCount;

        [SerializeField] private float shurikenSpawnDelay = 3f;
        [ReadOnly] private Coroutine _shurikenCoroutine;

        private void Update()
        {
            if (_shurikenCoroutine == null)
            {
                _shurikenCoroutine = StartCoroutine(ShootShurikens());
            }
        }

        private IEnumerator ShootShurikens()
        {
            yield return new WaitForSeconds(shurikenSpawnDelay);

        }

        public void ChooseTargets()
        {
            var availableTargets = towerBattleManager.GetAllActiveTowerRefs();
            for (int i = 0; i < shurikenTargetCount; i++)
            {
                // int index = Random.Range(0, availableTowerTargetIds.Count);
            }

        }
    }
}
