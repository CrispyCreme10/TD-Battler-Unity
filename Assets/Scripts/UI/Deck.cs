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
    [SerializeField] private VisualTreeAsset spellSlotAsset;
    [SerializeField] private List<CardData> playableCards;
    [SerializeField] private List<SpellData> playableSpells;

    private VisualElement _root;
    private ScrollView _availableCards;
    private ScrollView _availableSpells;
    private List<VisualElement> _selectedCardSlots;
    private VisualElement _selectedSpellSlot;
    private Button _playButton;

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _availableCards = _root.Q<ScrollView>("AvailableCards");
        _availableSpells = _root.Q<ScrollView>("AvailableSpells");
        _selectedCardSlots = _root.Q<VisualElement>("SelectedCards").Children().ToList();
        _selectedSpellSlot = _root.Q<VisualElement>("SpellSlot");
        _playButton = _root.Q<Button>("PlayBtn");
        _playButton.SetEnabled(false);

        List<CardData> playerSelectedCards = Singleton.Instance.PlayerManager.PlayerData.SelectedCards;
        SpellData playerSelectedSpell = Singleton.Instance.PlayerManager.PlayerData.SelectedSpell;
        
        // add all Available Cards to the Available Card List
        // set Visual Element data
        // grey out or disable all Available Cards that the player has Selected
        foreach (CardData card in playableCards)
        {
            VisualElement cardSlotElement = CreateCardSlot(card, playerSelectedCards.Contains(card));
            _availableCards.Add(cardSlotElement);
        }
        
        // add all Available Spells to the Available Spell List
        // set Visual Element data
        // grey out or disable the Spell the player has Selected
        foreach (SpellData spell in playableSpells)
        {
            VisualElement spellSlotElement = CreateSpellSlot(spell, spell == playerSelectedSpell);
            _availableSpells.Add(spellSlotElement);
        }
        
        // add all Selected Cards to Card Slots
        // set Visual Element data and make visible
        foreach (var (playerCard, index) in playerSelectedCards.WithIndex())
        {
            VisualElement currentCardSlot = _selectedCardSlots[index];
            currentCardSlot.Q<Label>("Name").text = playerCard.name;
            currentCardSlot.Q<VisualElement>("CardSlot").visible = true;
        }
        
        // add Selected Spell to Spell Slot
        // set Visual Element data and make visible
        if (playerSelectedSpell != null)
        {
            _selectedSpellSlot.Q<Label>("Name").text = playerSelectedSpell.name;
            _selectedSpellSlot.visible = true;
        }

        if (playerSelectedCards.Count > 0 && playerSelectedSpell != null)
        {
            _playButton.SetEnabled(true);
        }
    }
    
    private void OnAddSelectedCard(ClickEvent evt, CardData card)
    {
        // add card to player's selected card list
        // grey out/disable card visual element in the scroll view on deck screen
        // enable visual element in selected card slot corresponding to the position of the player's selected card
        
        Singleton.Instance.PlayerManager.PlayerData.AddSelectedCard(card);
    }
    
    private void OnRemoveSelectedCard(ClickEvent evt, CardData card)
    {
        Singleton.Instance.PlayerManager.PlayerData.RemoveSelectedCard(card);
    }

    private VisualElement CreateCardSlot(CardData card, bool setDisabled = false)
    {
        TemplateContainer templateContainer = cardSlotAsset.Instantiate();
        VisualElement cardSlotElement = cardSlotAsset.Instantiate();

        Label nameLabel = cardSlotElement.Q<Label>("Name");
        nameLabel.text = card.name;
        nameLabel.style.color = Color.white;
        cardSlotElement.visible = true;

        return cardSlotElement;
    }

    private VisualElement CreateSpellSlot(SpellData spell, bool setDisabled = false)
    {
        VisualElement spellSlotElement = spellSlotAsset.Instantiate();
        
        Label nameLabel = spellSlotElement.Q<Label>("Name");
        nameLabel.text = spell.name;
        nameLabel.style.color = Color.white;
        spellSlotElement.visible = true;

        return spellSlotElement;
    }
}