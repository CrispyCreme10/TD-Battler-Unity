using UnityEditor;
using UnityEngine;

namespace TDBattler.Editor
{
    [CustomEditor(typeof(Waypoint))]
    public class WaypointEditor : UnityEditor.Editor
    {
        private Waypoint Waypoint => target as Waypoint;
        private float _waypointSize = 0.7f;
        private float _waypointTextSize = 0.35f;

        private void OnSceneGUI()
        {
            if (Waypoint.hideDebug) return;

            if (Waypoint.Points.Count > 0)
            {
                Handles.color = Color.cyan;
                for (int i = 0; i < Waypoint.Points.Count; i++)
                {
                    EditorGUI.BeginChangeCheck();

                    // Create Handles
                    Vector3 currentWaypointPoint = Waypoint.CurrentPosition + Waypoint.Points[i];
                    Vector3 newWaypointPoint = Handles.FreeMoveHandle(currentWaypointPoint,
                        Quaternion.identity, _waypointSize * Waypoint.Scale.x,
                        new Vector3(0.3f, 0.3f, 0.3f), Handles.SphereHandleCap);

                    // Create text
                    GUIStyle textStyle = new GUIStyle();
                    textStyle.fontStyle = FontStyle.Bold;
                    textStyle.fontSize = 16;
                    textStyle.normal.textColor = Color.white;
                    Vector3 textAlignment = Vector3.down * _waypointTextSize * Waypoint.Scale.y + Vector3.right * _waypointTextSize * Waypoint.Scale.x;
                    Handles.Label(Waypoint.CurrentPosition + Waypoint.Points[i] + textAlignment,
                        $"{i + 1}", textStyle);
                    EditorGUI.EndChangeCheck();

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Free Move Handle");
                        Waypoint.Points[i] = newWaypointPoint - Waypoint.CurrentPosition;
                    }
                }
            }
        }
    }
}