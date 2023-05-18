using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Archer", fileName = "New Archer")]
public class ArcherScriptableObject : TowerScriptableObject, IDamageField, IAttackIntervalField, IAttackSpeedIncreaseField
{
    // [SerializeField] private ArcherFields baseLevel;

    // [SerializeField] private List<ArcherFields> mergeLevels;
    // [SerializeField] private List<ArcherFields> energyLevels;
    // [SerializeField] private List<ArcherFields> permanentLevels;

    // public float GetAttackInterval()
    // {
    //     return baseLevel.attackInterval + 
    //         mergeLevels[mergeLevel].attackInterval + 
    //         energyLevels[energyLevel].attackInterval + 
    //         permanentLevels[permanentLevel].attackInterval;
    // }

    // public int GetDamage()
    // {
    //     return baseLevel.damage + 
    //         mergeLevels[mergeLevel].damage + 
    //         energyLevels[energyLevel].damage + 
    //         permanentLevels[permanentLevel].damage;
    // }
    [field: SerializeField]
    public TowerFieldInt Damage { get; set; }

    public int DamageValue => Damage.GetValue(mergeLevel, energyLevel, permanentLevel);

    [field: SerializeField]
    public TowerFieldFloat AttackInterval { get; set; }

    public float AttackIntervalValue => AttackInterval.GetValue(mergeLevel, energyLevel, permanentLevel);

    [field: SerializeField]
    public TowerFieldFloat AttackSpeedIncrease { get; set; }

    public float AttackSpeedIncreaseValue => AttackSpeedIncrease.GetValue(mergeLevel, energyLevel, permanentLevel);
}

[Serializable]
public class ArcherFields : TowerFields
{
    public int damage;
    public float attackInterval;
    public float attackSpeedIncrease;
}

public interface ITowerField<T>
{
    public T BaseValue { get; set; }
    public List<T> MergeLevels { get; set; }
    public List<T> EnergyLevels { get; set; }
    public List<T> PermanentLevels { get; set; }
    public T GetValue(int mergeLevel, int energyLevel, int permanentLevel); 
}

[Serializable]
public class TowerFieldInt : ITowerField<int>
{
    [field: SerializeField]
    public int BaseValue { get; set; }
    [field: SerializeField]
    public List<int> MergeLevels { get; set; }
    [field: SerializeField]
    public List<int> EnergyLevels { get; set; }
    [field: SerializeField]
    public List<int> PermanentLevels { get; set; }

    public int GetValue(int mergeLevel, int energyLevel, int permanentLevel)
    {
        return BaseValue + MergeLevels[mergeLevel] + EnergyLevels[energyLevel] + PermanentLevels[permanentLevel];
    }
}

[Serializable]
public class TowerFieldFloat : ITowerField<float>
{
    [field: SerializeField]
    public float BaseValue { get; set; }
    [field: SerializeField]
    public List<float> MergeLevels { get; set; }
    [field: SerializeField]
    public List<float> EnergyLevels { get; set; }
    [field: SerializeField]
    public List<float> PermanentLevels { get; set; }

    public float GetValue(int mergeLevel, int energyLevel, int permanentLevel)
    {
        return BaseValue + MergeLevels[mergeLevel] + EnergyLevels[energyLevel] + PermanentLevels[permanentLevel];
    }
}

public interface IDamageField
{
    public TowerFieldInt Damage { get; set; }
}

public interface IAttackIntervalField
{
    public TowerFieldFloat AttackInterval { get; set; }
}

public interface IAttackSpeedIncreaseField
{
    public TowerFieldFloat AttackSpeedIncrease { get; set; }
}