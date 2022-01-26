using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Line))]
[CanEditMultipleObjects]
public class LineEditor : Editor
{
    SerializedProperty points;
    private void OnEnable()
    {
        points = serializedObject.FindProperty("points");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(points);
        serializedObject.ApplyModifiedProperties();
    }
}
