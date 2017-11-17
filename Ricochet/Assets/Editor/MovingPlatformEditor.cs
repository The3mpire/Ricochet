using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
    private MovingPlatform platform;

    private SerializedProperty useRandom;
    private SerializedProperty minWaitTime;
    private SerializedProperty moveSpeed;
    private SerializedProperty waitTime;
    private SerializedProperty positions;

    private void OnEnable()
    {
        platform = target as MovingPlatform;

        useRandom = serializedObject.FindProperty("useRandomWaitTime");
        minWaitTime = serializedObject.FindProperty("minWaitTime");
        moveSpeed = serializedObject.FindProperty("moveSpeed");
        waitTime = serializedObject.FindProperty("waitTime");
        positions = serializedObject.FindProperty("positions");

        if (platform.transform.hideFlags != HideFlags.HideInInspector)
        {
            platform.transform.hideFlags = HideFlags.HideInInspector;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(moveSpeed);
        EditorGUILayout.PropertyField(useRandom);
        EditorGUILayout.PropertyField(minWaitTime);
        EditorGUILayout.PropertyField(waitTime);
        EditorGUILayout.PropertyField(positions, true);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Add a Collider2D as a Trigger to enable \"squish\" mechanics."));

        platform.UpdatePosition();
    }

    private void OnSceneGUI()
    {
        List<Vector3> positions = platform.GetPositions();
        Handles.color = Color.magenta;

        if (positions == null || positions.Count == 0)
            return;
        
        Vector3[] lineSegments = new Vector3[positions.Count * 2];
        Vector3 prevPoint = positions[positions.Count - 1];

        int pointIndex = 0;
        for (int posInd = 0; posInd < positions.Count; posInd++)
        {
            Vector3 currPoint = positions[posInd];
            
            // store the starting point of the line segment
            lineSegments[pointIndex] = prevPoint;
            pointIndex++;

            // store the ending point of the line segment
            lineSegments[pointIndex] = currPoint;
            pointIndex++;

            prevPoint = currPoint;
        }
        Handles.DrawLines(lineSegments);
    }
}
