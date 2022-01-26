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
    /*public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(points);
        serializedObject.ApplyModifiedProperties();
    }*/

    private void OnSceneGUI()
    {
        Line line = (Line)target;
        for(int i = 0; i < line.points.Count; i++)
        {
            line.points[i] = Handles.PositionHandle(new Vector3(line.points[i].x, line.points[i].y, line.transform.position.z), Quaternion.identity);
        }
    }
}
