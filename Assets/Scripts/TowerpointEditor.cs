using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Towerpoint))]
public class TowerpointEditor : Editor
{
    private Towerpoint Towerpoint => target as Towerpoint;

    private void OnSceneGUI()
    {
        if (Towerpoint.Points.Length > 0)
        {
            Handles.color = Color.red;
            for (int i = 0; i < Towerpoint.Points.Length; i++)
            {
                EditorGUI.BeginChangeCheck();

                // Create Handles
                Vector3 currentWaypointPoint = Towerpoint.CurrentPosition + Towerpoint.Points[i];
                Vector3 newWaypointPoint = Handles.FreeMoveHandle(currentWaypointPoint,
                    Quaternion.identity, 0.7f,
                    new Vector3(0.3f, 0.3f, 0.3f), Handles.SphereHandleCap);

                // Create text
                GUIStyle textStyle = new GUIStyle();
                textStyle.fontStyle = FontStyle.Bold;
                textStyle.fontSize = 16;
                textStyle.normal.textColor = Color.white;
                Vector3 textAlignment = Vector3.down * 0.35f + Vector3.right * 0.35f;
                Handles.Label(Towerpoint.CurrentPosition + Towerpoint.Points[i] + textAlignment,
                    $"{i + 1}", textStyle);
                EditorGUI.EndChangeCheck();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Free Move Handle");
                    Towerpoint.Points[i] = newWaypointPoint - Towerpoint.CurrentPosition;
                }
            }
        }
    }
}
