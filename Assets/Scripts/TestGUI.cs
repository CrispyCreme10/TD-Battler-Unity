using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGUI : MonoBehaviour
{
    private void OnSceneGUI() 
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
        {
            Debug.Log("Clicked the button with text");
        }
    }
}
