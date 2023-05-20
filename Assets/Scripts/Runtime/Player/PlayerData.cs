using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    [SerializeField] private List<CardData> selectedCards;
    [SerializeField] private SpellData selectedSpell;

    public List<CardData> SelectedCards => selectedCards;
    public SpellData SelectedSpell => selectedSpell;

    public void AddSelectedCard(CardData card)
    {
        if (!selectedCards.Contains(card))
        {
            selectedCards.Add(card);
        }
    }

    public void RemoveSelectedCard(CardData card)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
        }
    }

    public void SetSelectedSpell(SpellData spell)
    {
        selectedSpell = spell;
    }

    public void RemoveSelectedSpell()
    {
        selectedSpell = null;
    }
}
