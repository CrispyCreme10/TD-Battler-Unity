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

    private VisualElement CreateCardVisualElement(CardData card)
    {
        StyleFloat borderWidth = new StyleFloat(2);
        StyleColor borderColor = new StyleColor(Color.blue);
        
        var cardElement = new VisualElement
        {
            style =
            {
                width = new StyleLength(200),
                height = new StyleLength(200),
                borderTopWidth = borderWidth,
                borderRightWidth = borderWidth,
                borderBottomWidth = borderWidth,
                borderLeftWidth = borderWidth,
                borderTopColor = borderColor,
                borderRightColor = borderColor,
                borderBottomColor = borderColor,
                borderLeftColor = borderColor
            }
        };
        
        Label cardLabel = new Label(card.name);
        cardElement.Add(cardLabel);

        return cardElement;
    }
    
    private void SetupAvailableCardList()
    {
        List<CardData> playerSelectedCards = Singleton.Instance.PlayerManager.GetSelectedCards();
        foreach (CardData card in playableCards)
        {
            VisualElement cardElement = CreateCardVisualElement(card);
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
            VisualElement cardElement = CreateCardVisualElement(card);
            _selectedCards[index].Add(cardElement);
            _selectedCards[index].RegisterCallback<ClickEvent, CardData>(OnRemoveSelectedCard, card);
        }
    }

    private void OnAddSelectedCard(ClickEvent evt, CardData card)
    {
        Singleton.Instance.PlayerManager.AddSelectedCard(card);
    }
    
    private void OnRemoveSelectedCard(ClickEvent evt, CardData card)
    {
        Singleton.Instance.PlayerManager.RemoveSelectedCard(card);
    }
}