using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    [CreateAssetMenu(menuName = "Enemy/Spawn Group", fileName = "New Spawn Group")]
    public class EnemySpawnGroups : ScriptableObject
    {
        [SerializeField] private List<SpawnGroup> spawnGroups;

        public List<SpawnGroup> SpawnGroups => spawnGroups;
    }

    [Serializable]
    public class SpawnGroup
    {
        public float groupDelay = 1f;
        public List<SpawnUnit> enemies;
        public Coroutine coroutine;
    }

    [Serializable]
    public struct SpawnUnit
    {
        public float initialDelay;
        public float enemyDelayGap;
        public int count;
        public EnemyPoolName poolName;
    }

    public enum EnemyPoolName
    {
        Grunt,
        Speeder,
        MiniBoss,
        Ninja
    }
}
