using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DocManager : MonoBehaviour
{
    [SerializeField] private UIDocument loginDocument;
    [SerializeField] private UIDocument deckDocument;
    [SerializeField] private UIDocument battleHudDocument;

    private void Start()
    {
        loginDocument.enabled = true;
    }
}
