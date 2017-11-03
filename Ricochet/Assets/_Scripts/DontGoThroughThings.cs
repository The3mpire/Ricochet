using UnityEngine;
using System.Collections;

public class DontGoThroughThings : MonoBehaviour
{
	#region Inspector Variables
	[SerializeField]
    [Tooltip("The layerMask this object is on")]
    private LayerMask layerMask = -1;
	[SerializeField]
    [Tooltip("How wide the objects outline is")]
    private float skinWidth = 0.1f;
    [SerializeField]
    [Tooltip("Drag the objects ridigBody2D here")]
    private Rigidbody2D rigid;
    [SerializeField]
    [Tooltip("Drag the objects collider here")]
    private Collider2D myCollider;

    #endregion

    #region Hidden Variables
    private float minimumExtent;
	private float partialExtent;
	private float sqrMinimumExtent;

	private Vector2 previousPosition;
	#endregion

	#region MonoBehavior
	void Start()
	{
		previousPosition = rigid.position;
		minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
		partialExtent = minimumExtent * (1.0f - skinWidth);
		sqrMinimumExtent = minimumExtent * minimumExtent;
	}

	void FixedUpdate()
	{
		Vector3 movementThisStep = rigid.position - previousPosition;
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
					rigid.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
				}
			}
		}
		previousPosition = rigid.position;
	}

	#endregion
}