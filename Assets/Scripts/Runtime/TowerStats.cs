using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    // responsible for holding information about active stat modifiers
    // put this component on an entity that wants modifiers applied to it
    public class StatRuntimeController : MonoBehaviour
    {
        public List<xStatModification> ActiveModifiers;

        public void AddModifier(xStatModification modifier)
        {
            // potentially 'Start' the modifier

            ActiveModifiers.Add(modifier);
        }

        public void RemoveModifier(xStatModification modifier)
        {
            ActiveModifiers.Remove(modifier);
        }
    }

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

    public class TowerStatsEntity
    {

    }

    public struct xStatModification
    {
        public xStatType StatToModify;
        public xStatModTypes ModificationType;
        public float ModificationValue;
        public float Timer;
        public xStatModFrequency ModificationFrequency;
        public xStatModSourceScope ModificationSourceScope;

        public static xStatModification Empty => new xStatModification 
        {
            StatToModify = xStatType.None,
            ModificationType = xStatModTypes.None,
            ModificationValue = 0f,
            Timer = 0f,
            ModificationFrequency = xStatModFrequency.Once,
            ModificationSourceScope = xStatModSourceScope.Single
        };
    }

    public enum xStatType
    {
        AttackDamage,
        MoveSpeed,
        None
    }
    
    public enum xStatModTypes
    {
        Percentage,
        Numerical, // +/- the float value 
        Absolute, // hard set the stat to this value
        None
    }

    public enum xStatModFrequency
    {
        Once,
        Continuous
    }

    public enum xStatModSourceScope
    {
        // i.e. 
        // Max of 50% slow to a single target from ALL Freezer towers on your side of the field 
        // vs 
        // every SINGLE tower can apply a Max of 50% slow to a single target
        Single, // the associated stat will apply mods based on source object instances
        All // the associated stat will apply mods based on the Type of the source object instance
    }
}