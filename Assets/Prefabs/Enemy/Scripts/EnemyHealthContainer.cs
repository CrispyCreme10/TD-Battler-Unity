using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealthContainer : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;

    public void SetHealthText(int newHealth)
    {
        textComponent.text = newHealth.ToString();
    }
}
