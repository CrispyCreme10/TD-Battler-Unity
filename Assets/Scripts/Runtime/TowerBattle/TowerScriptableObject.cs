using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower")]
public class TowerScriptableObject : SerializedScriptableObject
{
    public const int MAX_MERGE_LEVEL = 7;
    public const int MAX_ENERGY_LEVEL = 5;
    public const int MAX_PERMANENT_LEVEL = 15;

    [SerializeField] private new string name;
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private int energyLevel = 1;
    [SerializeField] private int permanentLevel = 1;
    [SerializeField] private TowerFaction towerFaction;
    [SerializeField] private UnitType unitType;
    [SerializeField] private UnitTarget target;
    [SerializeField] private float damage;
    [SerializeField] private float attackInterval;
    [SerializeField] private float heroEnergy;
    [SerializeField] private Dictionary<Stat, float> stats = new Dictionary<Stat, float>();
    [SerializeField] private Dictionary<StatModifier, float> statModifiers = new Dictionary<StatModifier, float>();
    [SerializeField] private Dictionary<Stat, List<float>> mergeStatLevels = new Dictionary<Stat, List<float>>();
    [SerializeField] private Dictionary<Stat, List<float>> energyStatLevels = new Dictionary<Stat, List<float>>();
    [SerializeField] private Dictionary<Stat, List<float>> permStatLevels = new Dictionary<Stat, List<float>>();
    [SerializeField] private Dictionary<StatModifier, List<float>> statModifierLevels = new Dictionary<StatModifier, List<float>>();
    [SerializeField] private Dictionary<StatModifier, List<float>> mergeModifierLevels = new Dictionary<StatModifier, List<float>>();
    [SerializeField] private Dictionary<StatModifier, List<float>> energyModifierLevels = new Dictionary<StatModifier, List<float>>();
    [SerializeField] private Dictionary<StatModifier, List<float>> permModifierLevels = new Dictionary<StatModifier, List<float>>();

    public string Name => name;
    public int EnergyLevel => energyLevel;
    public Dictionary<Stat, float> Stats => stats;

    private Dictionary<Stat, List<StatModifier>> statModifierMap = new Dictionary<Stat, List<StatModifier>>(){
        {Stat.AttackInterval, new List<StatModifier>(){ StatModifier.AttackSpeedIncrease }}
    };

    private void OnEnable() 
    {
        energyLevel = 1;
    }

    public float GetStat(Stat stat, int mergeLevel)
    {
        if (stats.TryGetValue(stat, out float value))
        {
            float baseVal = (value + 
                GetStatMergeLevelValue(stat, mergeLevel) + 
                GetStatEnergyLevelValue(stat, energyLevel) + 
                GetStatPermLevelValue(stat, permanentLevel));
            float val = GetModifierValues(stat, mergeLevel).Aggregate(baseVal, (total, val) => total * val);
            // Debug.Log($"GET STAT: {stat} {mergeLevel} {val} {this.name}");
            return val;
        }

        Debug.LogError($"No stat value found for {stat} on {this.name}");
        return 0;
    }

    public float GetStatMergeLevelValue(Stat stat, int mergeLevel)
    {
        if (mergeStatLevels.TryGetValue(stat, out List<float> levels))
        {
            int index = mergeLevel - 2;
            float val = index >= 0 && index < levels.Count ? levels[index] : 0;
            // Debug.Log($"MERGE LEVEL: {stat} {mergeLevel} {val} {this.name}");
            return val;
        }

        return 0;
    }

    public float GetStatEnergyLevelValue(Stat stat, int energyLevel)
    {
        if (energyStatLevels.TryGetValue(stat, out List<float> levels))
        {
            int index = energyLevel - 2;
            float val = index >= 0 && index < levels.Count ? levels[index] : 0;
            // Debug.Log($"ENERGY LEVEL: {stat} {energyLevel} {val} {this.name}");
            return val;
        }

        return 0;
    }

    public float GetStatPermLevelValue(Stat stat, int permLevel)
    {
        if (permStatLevels.TryGetValue(stat, out List<float> levels))
        {
            int index = permLevel - 2;
            float val = index >= 0 && index < levels.Count ? levels[index] : 0;
            // Debug.Log($"PERM LEVEL: {stat} {permLevel} {val} {this.name}");
            return val;
        }

        return 0;
    }

    public List<float> GetModifierValues(Stat stat, int mergeLevel)
    {
        if (statModifierMap.TryGetValue(stat, out List<StatModifier> modifiers))
        {
            return modifiers.Select((statModifier) => GetModifier(statModifier, mergeLevel)).ToList();
        }

        return new List<float> { 1 };
    }

    public float GetModifier(StatModifier statModifier, int mergeLevel)
    {
        if (statModifiers.TryGetValue(statModifier, out float value))
        {
            float val = (value + 
                    GetModMergeLevelValue(statModifier, mergeLevel) + 
                    GetModEnergyLevelValue(statModifier, energyLevel) + 
                    GetModPermLevelValue(statModifier, permanentLevel));
            // Debug.Log($"GET MOD: {statModifier} {mergeLevel} {val} {this.name}");
            return val;
        }
        else
        {
            Debug.LogError($"No mod value found for {statModifier} on {this.name}");
            return 1;
        }
    }

    public float GetModMergeLevelValue(StatModifier statMod, int mergeLevel)
    {
        if (mergeModifierLevels.TryGetValue(statMod, out List<float> levels))
        {
            int index = mergeLevel - 2;
            float val = index >= 0 && index < levels.Count ? levels[index] : 0;
            // Debug.Log($"MOD MERGE LEVEL: {stat} {mergeLevel} {val} {this.name}");
            return val;
        }

        return 0;
    }

    public float GetModEnergyLevelValue(StatModifier statMod, int energyLevel)
    {
        if (energyModifierLevels.TryGetValue(statMod, out List<float> levels))
        {
            int index = energyLevel - 2;
            float val = index >= 0 && index < levels.Count ? levels[index] : 0;
            // Debug.Log($"MOD ENERGY LEVEL: {stat} {energyLevel} {val} {this.name}");
            return val;
        }

        return 0;
    }

    public float GetModPermLevelValue(StatModifier statMod, int permLevel)
    {
        if (permModifierLevels.TryGetValue(statMod, out List<float> levels))
        {
            int index = permLevel - 2;
            float val = index >= 0 && index < levels.Count ? levels[index] : 0;
            // Debug.Log($"MOD PERM LEVEL: {stat} {permLevel} {val} {this.name}");
            return val;
        }

        return 0;
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