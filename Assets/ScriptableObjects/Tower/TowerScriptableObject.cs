using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower")]
public class TowerScriptableObject : SerializedScriptableObject
{
    public const int MAX_MERGE_LEVEL = 7;
    public const int MAX_ENERGY_LEVEL = 5;
    public const int MAX_PERMANENT_LEVEL = 15;

    public new string name;
    [TextArea]
    public string description;
    public int energyLevel = 1;
    public int permanentLevel = 1;
    public TowerFaction towerFaction;
    public UnitType unitType;
    public UnitTarget target;
    public float damage;
    public float attackInterval;
    public float heroEnergy;
    public Dictionary<Stat, float> stats = new Dictionary<Stat, float>();
    public Dictionary<StatModifier, float> statModifiers = new Dictionary<StatModifier, float>();
    public Dictionary<Stat, List<float>> statLevels = new Dictionary<Stat, List<float>>();
    public Dictionary<StatModifier, List<float>> statModifierLevels = new Dictionary<StatModifier, List<float>>();
}

// add/remove a Field from a Scriptable Object
// ability to check if a Scritable Object has a specific Field
// ability to retrieve a Fields value which has had the level modifiers applied
// load json file data into to avoid losing Scriptable Object data due to refactor

// want the ability to specify a group of non-base stats that a tower can have
// want the ability to upgrade 1..N grouping of stats for a tower (rn just for merge level & energy level increase/decrease)

public enum TowerFaction
{
    Forest,
    Magic,
    Light,
    Tech,
    Dark
}

public enum UnitType
{
    Damage,
    Debuff,
    Support,
    Special
}

public enum UnitTarget
{
    First,
    Random
}

public enum Stat
{
    Damage,
    AttackInterval,
    HeroEnergy
}

public enum StatModifier
{
    AttackSpeedIncrease
}