using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
