using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<CardData> playableCards;

    private VisualElement _root;
    
    private ListView _availableCards;
    private ListView _availableSpells;

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _availableCards = _root.Q<ListView>("AvailableCards");
        _availableSpells = _root.Q<ListView>("AvailableSpells");
        
        SetupAvailableCardList();
    }

    private void SetupAvailableCardList()
    {
        foreach (CardData card in playableCards)
        {
            VisualElement cardElement = new VisualElement
            {
                style =
                {
                    width = new StyleLength(50),
                    height = new StyleLength(50)
                }
            };

            Label cardLabel = new Label(card.name);
            cardElement.Add(cardLabel);
            
            _availableCards.Add(cardElement);
        }
    }
}
