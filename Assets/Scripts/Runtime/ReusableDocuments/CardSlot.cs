using UnityEngine;
using UnityEngine.UIElements;

namespace TDBattler.Runtime
{
    public class CardSlot : MonoBehaviour
    {
        [SerializeField] private static VisualTreeAsset _cardSlotAsset;

        public static VisualElement CreateCardSlot(CardData card)
        {
            VisualElement cardElement = _cardSlotAsset.Instantiate();


            return cardElement;
        }
    }
}