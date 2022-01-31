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
        Line line = (Line)target;
        if (GUILayout.Button("Reload"))
        {
            line.Reload();
        }
        if (GUILayout.Button("Clear Mesh"))
        {
            line.Clear();
        }

    }

    private void OnSceneGUI()
    {
        Line line = (Line)target;
        for(int i = 0; i < line.points.Count; i++)
        {
            Vector2 point = Handles.PositionHandle(
                new Vector3(
                    line.points[i].x * line.transform.lossyScale.x + line.transform.position.x,
                    line.points[i].y * line.transform.lossyScale.y + line.transform.position.y, 
                    line.transform.position.z + line.transform.position.z
                ), 
                Quaternion.identity
            ) 
            - line.transform.position;
            point /= line.transform.lossyScale;
            line.points[i] = point;
            Handles.color = Color.yellow;
            if(i != line.points.Count - 1)
            {
                Handles.DrawLine(line.points[i]*line.transform.lossyScale + (Vector2)line.transform.position, line.points[i + 1] * line.transform.lossyScale + (Vector2)line.transform.position);
            }
        }
    }
}
