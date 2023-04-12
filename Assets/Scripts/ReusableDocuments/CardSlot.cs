using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardSlot : MonoBehaviour
{
    [SerializeField] private static VisualTreeAsset _cardSlotAsset;

    public static VisualElement CreateCardSlot(CardData card)
    {
        VisualElement cardElement = _cardSlotAsset.Instantiate();


        return cardElement;
    }
}
