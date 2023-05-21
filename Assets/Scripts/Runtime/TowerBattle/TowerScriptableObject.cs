using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower")]
public class TowerScriptableObject : SerializedScriptableObject
{
    public static int MAX_MERGE_LEVEL = 7;
    public static int MAX_ENERGY_LEVEL = 5;
    public static int MAX_PERMANENT_LEVEL = 15;

    [SerializeField] private new string name;
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private Color debugColor;
    [SerializeField] private int energyLevel = 1;
    [SerializeField] private int permanentLevel = 1;
    [SerializeField] private TowerFaction towerFaction;
    [SerializeField] private UnitType unitType;
    [SerializeField] private UnitTarget target;
    [SerializeField] private float damage;
    [SerializeField] private float attackInterval;
    [SerializeField] private float heroEnergy;
    [SerializeField] private List<Stat> stats;

    public string Name => name;
    public Color DebugColor => debugColor;
    public int EnergyLevel => energyLevel;
    public int PermanentLevel => permanentLevel;
    public List<Stat> Stats => stats;

    private Dictionary<StatType, List<StatModifierType>> statModifierMap = new Dictionary<StatType, List<StatModifierType>>(){
        {StatType.AttackInterval, new List<StatModifierType>(){ StatModifierType.AttackSpeedIncrease }}
    };

    private void OnEnable() 
    {
        energyLevel = 1;
    }

    public int IncrementEnergyLevel()
    {
        energyLevel++;
        Debug.Log($"Increase energy level {energyLevel} {this.name}");
        return energyLevel;
    }
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
    Random,
    None,
}

public enum StatType
{
    Damage,
    AttackInterval,
    HeroEnergy
}

public enum StatModifierType
{
    AttackSpeedIncrease
}