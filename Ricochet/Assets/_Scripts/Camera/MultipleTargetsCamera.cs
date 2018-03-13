using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetsCamera : MonoBehaviour
{
	#region Inspector Variables
	[Header("XY Movement Settings")]
	[SerializeField] private List<Transform> targets;
	[SerializeField] private Vector3 offset;
	[SerializeField] private float smoothTime = .5f;
	[SerializeField] private Vector2 minPos;
	[SerializeField] private Vector2 maxPos;

	[Header("Zoom Out")]
	[SerializeField] private float zoomOutTime;
	[SerializeField] private float zoomInTime;

	[Header("Manual Sizing Options")]
	[SerializeField] private bool manualZooming;
	[SerializeField] private float initialZoom = 0f;
	[SerializeField] private float maxZoomOut;
	[SerializeField] private float zoomRatio;
	[SerializeField] private Vector2 zoomThresholds;

	[Header("Level Wall Transforms")]
	[Tooltip("Drag the top outer wall here")]
	[SerializeField] private Transform topWallTransform;
	[Tooltip("Drag the bottom outer wall here")]
	[SerializeField] private Transform bottomWallTransform;
	[Tooltip("Drag the left outer wall here")]
	[SerializeField] private Transform leftWallTransform;
	[Tooltip("Drag the right outer wall here")]
	[SerializeField] private Transform rightWallTransform;
	#endregion

	#region Private Variables
	private Camera camera;
	private GameManager manager;
	private Vector3 velocity;
    
	// zoom variables
	private Transform[] outerWalls;
	private Vector2 minZoomPos;
	private Vector2 maxZoomPos;

	private float zoomVel;
	private float aspectRatio;
	#endregion

	#region Monobehaviours

	public void Awake()
	{
		camera = gameObject.GetComponent<Camera>();
		aspectRatio = camera.aspect;
		GameObject center = new GameObject("Center Transform");
		center.transform.parent = transform;
		center.transform.position = Vector3.zero;
		targets.Add(center.transform);

		outerWalls = new Transform[] { topWallTransform, bottomWallTransform, leftWallTransform, rightWallTransform };
		SetZoom(aspectRatio, outerWalls);
		//initialZoom = camera.orthographicSize;
		camera.orthographicSize = initialZoom;
	}

	public void Start()
	{
		if(!GameManager.TryGetInstance(out manager))
		{
			Debug.LogError("Camera unable to find GameManager", gameObject);
			return;
		}
        
		List<GameObject> balls = manager.GetBallObjects();

		foreach(PlayerController p in manager.GetPlayers())
		{
			targets.Add(p.transform);
		}
		foreach(GameObject go in balls)
		{
			targets.Add(go.transform);
		}
	}

	public void LateUpdate()
	{
		if(targets.Count == 0)
		{
			return;
		}

		Bounds targetBounds = GetTargetBounds();
		Vector3 centerPoint = targetBounds.center;

		float x = Mathf.Clamp(centerPoint.x, minZoomPos.x, maxZoomPos.x);
		float y = Mathf.Clamp(centerPoint.y, minZoomPos.y, maxZoomPos.y);

		Vector3 targetPoint = new Vector3(x, y) + offset;

		transform.position = Vector3.SmoothDamp(transform.position, targetPoint, ref velocity, smoothTime);

		if(targetBounds.extents.x > zoomThresholds.x)
		{
			float z = initialZoom + (zoomRatio / 100) * (targetBounds.extents.x - zoomThresholds.x);
			z = Mathf.Clamp(z, initialZoom, initialZoom + maxZoomOut);
			float smoothZ = Mathf.SmoothDamp(camera.orthographicSize, z, ref zoomVel, zoomOutTime);
			camera.orthographicSize = smoothZ;
		}
		else if(targetBounds.extents.y > zoomThresholds.y)
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
		if(targets.Count == 1)
		{
			return new Bounds(targets[0].position, targets[0].position);
		}

		Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
		for(int i = 0; i < targets.Count; i++)
		{
			if(targets[i].gameObject.activeSelf)
			{
				bounds.Encapsulate(targets[i].position);
			}
		}

		return bounds;
	}

	#endregion

	#region Public Helpers

	public void AddTarget(Transform t)
	{
		targets.Add(t);
	}

	public void RemoveTarget(Transform t)
	{
		targets.Remove(t);
	}

	#endregion

	#region Private Functions
	private void SetZoom(float aspectRatio, Transform[] outerWalls)
	{
		float hHalfDistance = Mathf.Abs(outerWalls[3].localPosition.x - outerWalls[2].localPosition.x) / 2;
		float vHalfDistance = Mathf.Abs(outerWalls[1].localPosition.y - outerWalls[0].localPosition.y) / 2;
		float sceneRatio = hHalfDistance / vHalfDistance;

		// automatically set zoom based on level dimensions
		if(!manualZooming)
		{
			if(sceneRatio > aspectRatio)
			{
				Debug.Log("Horizontal Level");
				initialZoom = vHalfDistance;
				maxZoomOut = hHalfDistance / aspectRatio - initialZoom;
				zoomThresholds[0] = initialZoom / 1.1f * aspectRatio - 3;
				zoomThresholds[1] = initialZoom;
				maxPos[0] = Mathf.Abs(hHalfDistance - initialZoom * aspectRatio);
				minPos[0] = -maxPos[0];
				zoomRatio = maxPos[0] / maxZoomOut * 100;
			}
			else
			{
				Debug.Log("Vertical Level");
				initialZoom = hHalfDistance / aspectRatio;
				maxZoomOut = vHalfDistance - initialZoom;
				zoomThresholds[0] = initialZoom * aspectRatio;
				zoomThresholds[1] = initialZoom / 1.1f - 3;
				maxPos[1] = Mathf.Abs(vHalfDistance - initialZoom);
				minPos[1] = -maxPos[1];
				// set other dimension if not autozoom
				zoomRatio = maxPos[1] / maxZoomOut * 100;
			}
		}

		minZoomPos = minPos;
		maxZoomPos = maxPos;
	}
	#endregion
}
