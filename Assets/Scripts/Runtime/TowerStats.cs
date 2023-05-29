using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public class StatModificationSystem
    {
        public void Recalculate(Tower towerRef)
        {

        }
    }

    public class EnemyStatsEntity
    {
        public float Damage;
        public float MovementSpeed;
    }

    public class TownStatsEntity
    {

    }

    public struct StatModification
    {
        public xStatType StatToModify;
        public xStatModificationTypes ModificationType;
        public float ModificationValue;
        public float Timer;

        public static StatModification Empty => new StatModification 
        {
            StatToModify = xStatType.None,
            ModificationType = xStatModificationTypes.None,
            ModificationValue = 0f,
            Timer = 0f
        };
    }

    public enum xStatType
    {
        AttackDamage,
        MoveSpeed,
        None
    }
    
    public enum xStatModificationTypes
    {
        Percentage,
        Numerical, // +/- the float value 
        Absolute, // hard set the stat to this value
        None
    }
}