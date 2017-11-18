using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Shield))]
public class ShieldEditor : Editor
{
    private Shield bezierCollider;
    private EdgeCollider2D edgeCollider;
    private LineRenderer lineRenderer;

    private SerializedProperty firstPoint;
    private SerializedProperty secondPoint;
    private SerializedProperty startPointHandler;
    private SerializedProperty endPointHandler;

    private SerializedProperty colliderMaterial;
    private SerializedProperty colliderEdgeRadius;

    private SerializedProperty lineRendererMaterial;
    private SerializedProperty lineRendererWidth;
    private SerializedProperty lineRendererColor;

    private SerializedProperty numberOfPoints;

    private SerializedProperty playerController;


    private GUIStyle header;

    private void OnEnable()
    {
        playerController = serializedObject.FindProperty("playerController");

        colliderMaterial = serializedObject.FindProperty("colliderMaterial");
        colliderEdgeRadius = serializedObject.FindProperty("colliderEdgeRadius");

        lineRendererMaterial = serializedObject.FindProperty("lineRendererMaterial");
        lineRendererWidth = serializedObject.FindProperty("lineRendererWidth");
        lineRendererColor = serializedObject.FindProperty("lineRendererColor");

        numberOfPoints = serializedObject.FindProperty("numberOfPoints");

        firstPoint = serializedObject.FindProperty("startPoint");
        secondPoint = serializedObject.FindProperty("endPoint");
        startPointHandler = serializedObject.FindProperty("startPointHandler");
        endPointHandler = serializedObject.FindProperty("endPointHandler");

        header = new GUIStyle()
        {
            fontStyle = FontStyle.Bold
        };
    }

    public override void OnInspectorGUI()
    {
        bezierCollider = (Shield)target;
        edgeCollider = bezierCollider.GetComponent<EdgeCollider2D>();
        lineRenderer = bezierCollider.GetComponent<LineRenderer>();

        if (edgeCollider == null)
        {
            Debug.LogError(name + " is missing required components: " + typeof(EdgeCollider2D).ToString(), bezierCollider.gameObject);
            return;
        }
        if (lineRenderer == null)
        {
            Debug.LogError(name + " is missing required components: " + typeof(LineRenderer).ToString(), bezierCollider.gameObject);
            return;
        }

        if (edgeCollider.hideFlags != HideFlags.HideInInspector)
        {
            edgeCollider.hideFlags = HideFlags.HideInInspector;
        }

        if (lineRenderer.hideFlags != HideFlags.HideInInspector)
        {
            lineRenderer.hideFlags = HideFlags.HideInInspector;
        }

        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Shield"), header);
        EditorGUILayout.PropertyField(playerController);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Shield Curve"), header);
        EditorGUILayout.PropertyField(numberOfPoints);
        EditorGUILayout.PropertyField(firstPoint);
        EditorGUILayout.PropertyField(secondPoint);
        EditorGUILayout.PropertyField(startPointHandler);
        EditorGUILayout.PropertyField(endPointHandler);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Edge Collider Properties"), header);
        EditorGUILayout.PropertyField(colliderMaterial);
        EditorGUILayout.PropertyField(colliderEdgeRadius);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Line Renderer Properties"), header);
        EditorGUILayout.PropertyField(lineRendererMaterial);
        EditorGUILayout.PropertyField(lineRendererWidth);
        EditorGUILayout.PropertyField(lineRendererColor);
        serializedObject.ApplyModifiedProperties();
        bezierCollider.RefreshShield();
    }

    private void OnSceneGUI()
    {
        if (bezierCollider == null)
            return;

        Vector3 pos = bezierCollider.transform.position;

        Handles.color = Color.cyan;
        Handles.DrawLine(pos + (Vector3)bezierCollider.GetStartPointHandler(), pos + (Vector3)bezierCollider.GetStartPoint());
        Handles.DrawLine(pos + (Vector3)bezierCollider.GetEndPointHandler(), pos + (Vector3)bezierCollider.GetEndPoint());

        Vector2 start = Handles.FreeMoveHandle(pos + (Vector3)bezierCollider.GetStartPoint(), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(pos + (Vector3)bezierCollider.GetStartPoint()), Vector3.zero, Handles.DotHandleCap) - pos;
        Vector2 end = Handles.FreeMoveHandle(pos + (Vector3)bezierCollider.GetEndPoint(), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(pos + (Vector3)bezierCollider.GetEndPoint()), Vector3.zero, Handles.DotHandleCap) - pos;
        Vector2 startH = Handles.FreeMoveHandle(pos + (Vector3)bezierCollider.GetStartPointHandler(), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(pos + (Vector3)bezierCollider.GetStartPointHandler()), Vector3.zero, Handles.DotHandleCap) - pos;
        Vector2 endH = Handles.FreeMoveHandle(pos + (Vector3)bezierCollider.GetEndPointHandler(), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(pos + (Vector3)bezierCollider.GetEndPointHandler()), Vector3.zero, Handles.DotHandleCap) - pos;

        bezierCollider.BuildShield(-1, start, end, startH, endH);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}
