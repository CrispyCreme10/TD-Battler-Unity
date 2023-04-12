using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class Deck : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset cardSlotAsset;
    [SerializeField] private List<CardData> playableCards;

    private VisualElement _root;
    
    private ScrollView _availableCards;
    private ScrollView _availableSpells;

    private List<VisualElement> _selectedCards;

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _availableCards = _root.Q<ScrollView>("AvailableCards");
        SetupAvailableCardList();
        
        _availableSpells = _root.Q<ScrollView>("AvailableSpells");

        _selectedCards = _root.Q<VisualElement>("SelectedCards").Children().ToList();
        SetupSelectedCards();
        
    }
    
    private void SetupAvailableCardList()
    {
        List<CardData> playerSelectedCards = Singleton.Instance.PlayerManager.GetSelectedCards();
        foreach (CardData card in playableCards)
        {
            VisualElement cardElement = CreateCardSlot(card);
            if (!playerSelectedCards.Contains(card))
            {
                cardElement.RegisterCallback<ClickEvent, CardData>(OnAddSelectedCard, card);
            }
            
            _availableCards.Add(cardElement);
        }
    }

    private void SetupSelectedCards()
    {
        foreach (var (card, index) in Singleton.Instance.PlayerManager.GetSelectedCards().WithIndex())
        {
            VisualElement cardElement = CreateCardSlot(card);
            _selectedCards[index].Add(cardElement);
            _selectedCards[index].RegisterCallback<ClickEvent, CardData>(OnRemoveSelectedCard, card);
        }
    }
    
    private void OnAddSelectedCard(ClickEvent evt, CardData card)
    {
        // add card to player's selected card list
        // grey out/disable card visual element in the scroll view on deck screen
        // enable visual element in selected card slot corresponding to the position of the player's selected card
        
        Singleton.Instance.PlayerManager.AddSelectedCard(card);
    }
    
    private void OnRemoveSelectedCard(ClickEvent evt, CardData card)
    {
        Singleton.Instance.PlayerManager.RemoveSelectedCard(card);
    }

    private VisualElement CreateCardSlot(CardData card)
    {
        VisualElement cardElement = cardSlotAsset.Instantiate();
        
        

        return cardElement;
    }
}