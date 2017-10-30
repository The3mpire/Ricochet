using UnityEngine;
using System.Collections;

public class DontGoThroughThings : MonoBehaviour
{
	#region Inspector Variables
	[SerializeField]
	private bool sendTriggerMessage = false;
	[Tooltip("")]
	[SerializeField]
	private LayerMask layerMask = -1;
	[Tooltip("")]
	[SerializeField]
	private float skinWidth = 0.1f;
	[Tooltip("")]

	#endregion

	#region Hidden Variables
	private float minimumExtent;
	private float partialExtent;
	private float sqrMinimumExtent;

	private Vector3 previousPosition;

	private Rigidbody myRigidbody;

	private Collider myCollider;

	#endregion

	#region MonoBehavior
	void Start()
	{
		myRigidbody = GetComponent<Rigidbody>();
		myCollider = GetComponent<Collider>();
		previousPosition = myRigidbody.position;
		minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
		partialExtent = minimumExtent * (1.0f - skinWidth);
		sqrMinimumExtent = minimumExtent * minimumExtent;
	}

	void FixedUpdate()
	{
		Vector3 movementThisStep = myRigidbody.position - previousPosition;
		float movementSqrMagnitude = movementThisStep.sqrMagnitude;

		if (movementSqrMagnitude > sqrMinimumExtent)
		{
			float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);

			RaycastHit hitInfo;
			if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
			{
				if (!hitInfo.collider)
				{
					return;
				}
				if (hitInfo.collider.isTrigger)
				{
					hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);
				}
				if (!hitInfo.collider.isTrigger)
				{
					myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
				}
			}
		}
		previousPosition = myRigidbody.position;
	}

	#endregion
}