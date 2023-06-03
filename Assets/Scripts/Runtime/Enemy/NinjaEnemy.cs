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
        [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected Transform firingPoint;

        [SerializeField] private int shurikenTargetCount;

        [SerializeField] private float shurikenSpawnDelay = 3f;
        [SerializeField] private float delayBetweenShurikens = 0.5f;
        [ReadOnly] private Coroutine _shurikenCoroutine;
        private GameObject _projectileContainer;

        private void Awake()
        {
            _projectileContainer = new GameObject("Projectiles");
            _projectileContainer.transform.parent = transform;
        }

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

            var availableTargets = towerBattleManager.GetAllActiveTowerRefs();
            int adjustedShurikenCount = Mathf.Min(shurikenTargetCount, availableTargets.Count);
            for (int i = 0; i < adjustedShurikenCount; i++)
            {
                int index = Random.Range(0, availableTargets.Count);
                yield return ShootShuriken(availableTargets[index]);
            }

            _shurikenCoroutine = null;
        }

        private IEnumerator ShootShuriken(GameObject targetObj)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, Quaternion.identity, _projectileContainer.transform);
            Projectile projectileScript = projectileObj.GetComponent<Projectile>();
            projectileScript.SetSourceObjectName(name);
            projectileScript.SetTarget(targetObj);
            projectileObj.name = $"{name} - {projectileObj.name}";
            yield return new WaitForSeconds(delayBetweenShurikens);
        }
    }
}
