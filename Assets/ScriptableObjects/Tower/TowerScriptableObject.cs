using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerScriptableObject : ScriptableObject
{
    protected const int MAX_MERGE_LEVEL = 7;
    protected const int MAX_ENERGY_LEVEL = 5;
    protected const int MAX_PERMANENT_LEVEL = 15;

    [SerializeField] protected new string name;
    [TextArea]
    [SerializeField] protected string description;
    [SerializeField] protected int permanentLevel = 1;
    [SerializeField] protected TowerFaction towerFaction;
    [SerializeField] protected UnitType unitType;
    [SerializeField] protected UnitTarget target;
}

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

public class TowerFields
{
    [SerializeField] private float heroEnergy;
}