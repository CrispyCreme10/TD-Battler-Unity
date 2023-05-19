using UnityEngine;

namespace TDBattler.Runtime
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject loginDocumentObj;
        [SerializeField] private GameObject deckDocument;
        [SerializeField] private GameObject battleHudDocument;

        private void Start()
        {
            deckDocument.SetActive(true);
        }
    }
}