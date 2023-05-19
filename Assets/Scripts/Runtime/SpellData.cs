using UnityEngine;

[CreateAssetMenu]
public class SpellData : ScriptableObject
{
    public new string name = "New Card";
    public string description = "This is the description of a new spell.";
    public int level = 1;
    public int xp = 0;
    
    // battle info
    public int damage = 0;
}
