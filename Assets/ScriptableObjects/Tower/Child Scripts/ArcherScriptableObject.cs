using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Archer", fileName = "New Archer")]
public class ArcherScriptableObject : TowerScriptableObject
{
    [SerializeField] private ArcherFields baseLevel;

    [SerializeField] private List<ArcherFields> mergeLevels;
    [SerializeField] private List<ArcherFields> energyLevels;
    [SerializeField] private List<ArcherFields> permanentLevels;
}

[Serializable]
public class ArcherFields : TowerFields
{
    [SerializeField] private int damage;
    [SerializeField] private float attackInterval;
    [SerializeField] private float attackSpeedIncrease;
}