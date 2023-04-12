using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardData : ScriptableObject
{
    public new string name = "New Card";
    public string description = "This is the description of a new card.";
    public int level = 1;
    public int xp = 0;
    
    // battle info
    public int damage = 0;
    public Target target = Target.First;
    public float attackSpeed = 1.0f;
}

public enum Target
{
    First,
    Last,
    Strongest,
    Random
}