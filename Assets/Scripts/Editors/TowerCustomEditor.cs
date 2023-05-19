using TDBattler.Runtime;
using UnityEditor;
using UnityEngine;

namespace TDBattler.Editor
{
    [CustomEditor(typeof(Tower))]
    public class TowerCustomEditor : UnityEditor.Editor
    {
        private Tower t;

        private SerializedProperty m_mergeLevel;

        private void OnEnable()
        {
            t = target as Tower;
            m_mergeLevel = serializedObject.FindProperty("_mergeLevel");
        }

        public override void OnInspectorGUI()
        {

            // read-only display
            GUILayout.Label("Read-Only Values", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            m_mergeLevel.intValue = EditorGUILayout.IntField("Merge Level", m_mergeLevel.intValue);
            EditorGUI.EndDisabledGroup();
        }
    }
}