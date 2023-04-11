using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    public List<CardData> GetSelectedCards()
    {
        return playerData.GetSelectedCards();
    }
    
    public void AddSelectedCard(CardData card)
    {
        playerData.AddSelectedCard(card);
    }

    public void RemoveSelectedCard(CardData card)
    {
        playerData.RemoveSelectedCard(card);
    }
}
