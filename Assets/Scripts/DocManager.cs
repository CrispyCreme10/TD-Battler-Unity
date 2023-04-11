using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DocManager : MonoBehaviour
{
    [SerializeField] private GameObject loginDocumentObj;
    [SerializeField] private UIDocument deckDocument;
    [SerializeField] private UIDocument battleHudDocument;

    private void Start()
    {
        loginDocumentObj.SetActive(true);
    }
}
