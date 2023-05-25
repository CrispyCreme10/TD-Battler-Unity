using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System;

namespace TDBattler.Runtime
{
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
        [SerializeField] private List<Stat> stats;

        public string Name => name;
        public Color DebugColor => debugColor;
        public int EnergyLevel => energyLevel;
        public int PermanentLevel => permanentLevel;
        public UnitType UnitType => unitType;
        public UnitTarget UnitTarget => target;
        public List<Stat> Stats => stats;

        private Dictionary<StatType, List<StatModifierType>> statModifierMap = new Dictionary<StatType, List<StatModifierType>>(){
            {StatType.AttackInterval, new List<StatModifierType>(){ StatModifierType.AttackSpeedIncrease }}
        };

        private void OnEnable() 
        {
            energyLevel = 1;
        }

        public void IncrementEnergyLevel()
        {
            energyLevel++; 
        }
    }

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
        HeroEnergy,
        GenerateMana
    }

    public enum StatModifierType
    {
        AttackSpeedIncrease
    }
}