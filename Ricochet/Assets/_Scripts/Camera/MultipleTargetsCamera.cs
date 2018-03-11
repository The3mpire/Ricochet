using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetsCamera : MonoBehaviour
{
    [Header("XY Movement Settings")]
    [SerializeField] private List<Transform> targets;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothTime = .5f;
    [SerializeField] private Vector2 minPos;
    [SerializeField] private Vector2 maxPos;

    [Header("Zoom Out")]
    [SerializeField] private float zoomOutTime;
    [SerializeField] private float zoomInTime;

    [Header("Level Wall Transforms")]
    [SerializeField] private Transform topWallTransform;
    [SerializeField] private Transform bottomWallTransform;
    [SerializeField] private Transform leftWallTransform;
    [SerializeField] private Transform rightWallTransform;


    private Camera camera;
    private GameManager manager;
    private Vector3 velocity;
    
    // zoom variables
    private Transform[] outerWalls;
    private Vector2 minZoomPos;
    private Vector2 maxZoomPos;
    private Vector2 zoomThresholds;
    private float initialZoom;
    private float maxZoomOut;
    private float zoomRatio;
    private float zoomVel;
    private float aspectRatio;

    #region Monobehaviours
    public void Awake()
    {
        camera = gameObject.GetComponent<Camera>();
        aspectRatio = camera.aspect;
        GameObject center = new GameObject("Center Transform");
        center.transform.parent = transform;
        center.transform.position = Vector3.zero;
        targets.Add(center.transform);

        outerWalls = new Transform[] {topWallTransform, bottomWallTransform, leftWallTransform, rightWallTransform};
        SetZoom(aspectRatio, outerWalls);
        //initialZoom = camera.orthographicSize;
        camera.orthographicSize = initialZoom;
    }

    public void Start()
    {
        if (!GameManager.TryGetInstance(out manager))
        {
            Debug.LogError("Camera unable to find GameManager", gameObject);
            return;
        }
        
        List<GameObject> balls = manager.GetBallObjects();

        foreach (PlayerController p in manager.GetPlayers())
        {
            targets.Add(p.transform);
        }
        foreach (GameObject go in balls)
        {
            targets.Add(go.transform);
        }
    }

    public void LateUpdate()
    {
        if (targets.Count == 0)
        {
            return;
        }

        Bounds targetBounds = GetTargetBounds();
        Vector3 centerPoint = targetBounds.center;

        float x = Mathf.Clamp(centerPoint.x, minZoomPos.x, maxZoomPos.x);
        float y = Mathf.Clamp(centerPoint.y, minZoomPos.y, maxZoomPos.y);

        Vector3 targetPoint = new Vector3(x, y) + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPoint, ref velocity, smoothTime);

        if (targetBounds.extents.x > zoomThresholds.x)
        {
            float z = initialZoom + (zoomRatio / 100) * (targetBounds.extents.x - zoomThresholds.x);
            z = Mathf.Clamp(z, initialZoom, initialZoom + maxZoomOut);
            float smoothZ = Mathf.SmoothDamp(camera.orthographicSize, z, ref zoomVel, zoomOutTime);
            camera.orthographicSize = smoothZ;
        }
        else if (targetBounds.extents.y > zoomThresholds.y)
        {
            float z = initialZoom + (zoomRatio / 100) * (targetBounds.extents.y - zoomThresholds.y);
            z = Mathf.Clamp(z, initialZoom, initialZoom + maxZoomOut);
            float smoothZ = Mathf.SmoothDamp(camera.orthographicSize, z, ref zoomVel, zoomOutTime);
            camera.orthographicSize = smoothZ;
        }
        else
        {
            float smoothZ = Mathf.SmoothDamp(camera.orthographicSize, initialZoom, ref zoomVel, zoomInTime);
            camera.orthographicSize = smoothZ;
        }

        minZoomPos = minPos - minPos * (camera.orthographicSize - initialZoom) / maxZoomOut;
        maxZoomPos = maxPos - maxPos * (camera.orthographicSize - initialZoom) / maxZoomOut;
    }
    #endregion

    #region Getters
    public Bounds GetTargetBounds()
    {
        if (targets.Count == 1)
        {
            return new Bounds(targets[0].position, targets[0].position);
        }

        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].gameObject.activeSelf)
            {
                bounds.Encapsulate(targets[i].position);
            }
        }

        return bounds;
    }
    #endregion

    private void SetZoom(float aspectRatio, Transform[] outerWalls)
    {
        float hHalfDistance = Mathf.Abs(outerWalls[3].localPosition.x - outerWalls[2].localPosition.x) / 2;
        float vHalfDistance = Mathf.Abs(outerWalls[1].localPosition.y - outerWalls[0].localPosition.y) / 2;
        float sceneRatio = hHalfDistance / vHalfDistance;
        if (sceneRatio > aspectRatio)
        {
            initialZoom = vHalfDistance;
            maxZoomOut = (hHalfDistance - initialZoom) * aspectRatio / 2;
            zoomThresholds[0] = initialZoom / 1.1f * aspectRatio - 3 ;
            zoomThresholds[1] = vHalfDistance;
            maxPos[0] = Mathf.Abs(hHalfDistance - initialZoom * aspectRatio);
            minPos[0] = -maxPos[0];
            zoomRatio = maxPos[0] / maxZoomOut * 100;
        }
        else
        {
            initialZoom = hHalfDistance / aspectRatio;
            maxZoomOut = (vHalfDistance / aspectRatio - initialZoom) / 2;
            zoomThresholds[0] = hHalfDistance;
            zoomThresholds[1] = initialZoom / 1.1f - 3;
            maxPos[1] = Mathf.Abs(vHalfDistance - initialZoom);
            minPos[1] = -maxPos[1];
            zoomRatio = maxPos[1] / maxZoomOut * 100;
        }
        minZoomPos = minPos;
        maxZoomPos = maxPos;
    }
}
