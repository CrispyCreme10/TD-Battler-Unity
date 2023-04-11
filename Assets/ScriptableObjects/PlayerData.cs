using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    [SerializeField] private List<CardData> selectedCards;

    public List<CardData> GetSelectedCards()
    {
        return selectedCards;
    }
    
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
}
