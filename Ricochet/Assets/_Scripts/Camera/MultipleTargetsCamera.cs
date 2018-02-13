using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetsCamera : MonoBehaviour
{

    [SerializeField] private List<Transform> targets;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothTime = .5f;
    [SerializeField] private Vector2 minPos;
    [SerializeField] private Vector2 maxPos;
    [SerializeField] private float maxZoomOut = 2f;
    [SerializeField] private float zoomOutTime = 0.4f;
    [SerializeField] private float zoomInTime = 0.7f;

    private Camera camera;
    private GameManager manager;
    private Vector3 velocity;
    private float zoomVel;
    private float initialZoom;

    #region Monobehaviours
    public void Awake()
    {
        camera = gameObject.GetComponent<Camera>();
        GameObject center = new GameObject("Center Transform");
        center.transform.parent = transform;
        center.transform.position = Vector3.zero;
        targets.Add(center.transform);
        initialZoom = camera.orthographicSize;
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

        float x = Mathf.Clamp(centerPoint.x, minPos.x, maxPos.x);
        float y = Mathf.Clamp(centerPoint.y, minPos.y, maxPos.y);

        Vector3 targetPoint = new Vector3(x, y) + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPoint, ref velocity, smoothTime);

        if (targetBounds.extents.x > 2.25*maxPos.x)
        {
            float z = initialZoom + Mathf.Abs(targetBounds.extents.x - maxPos.x);
            z = Mathf.Clamp(z, initialZoom, initialZoom + maxZoomOut);
            float smoothZ = Mathf.SmoothDamp(camera.orthographicSize, z, ref zoomVel, zoomOutTime);
            camera.orthographicSize = smoothZ;
        }
        else
        {
            float smoothZ = Mathf.SmoothDamp(camera.orthographicSize, initialZoom, ref zoomVel, zoomInTime);
            camera.orthographicSize = smoothZ;
        }
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
}
