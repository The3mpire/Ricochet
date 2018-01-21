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

    private GameManager manager;
    private Vector3 velocity;

    #region Monobehaviours
    public void Awake()
    {
        GameObject center = new GameObject("Center Transform");
        center.transform.parent = transform;
        center.transform.position = Vector3.zero;
        targets.Add(center.transform);
    }

    public void Start()
    {
        if (!GameManager.TryGetInstance(out manager))
        {
            Debug.LogError("Camera unable to find GameManager", gameObject);
            return;
        }
        
        GameObject[] players = manager.GetPlayerObjects();
        List<GameObject> balls = manager.GetBallObjects();

        foreach (GameObject go in players)
        {
            targets.Add(go.transform);
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

        Vector3 centerPoint = GetCenterPoint();

        float x = Mathf.Clamp(centerPoint.x, minPos.x, maxPos.x);
        float y = Mathf.Clamp(centerPoint.y, minPos.y, maxPos.y);

        Vector3 targetPoint = new Vector3(x, y) + offset;
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPoint, ref velocity, smoothTime);
    }
    #endregion

    #region Getters
    public Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].gameObject.activeSelf)
            {
                bounds.Encapsulate(targets[i].position);
            }
        }

        return bounds.center;
    }
    #endregion
}
