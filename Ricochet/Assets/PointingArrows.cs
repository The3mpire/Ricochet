using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingArrows : MonoBehaviour 
{

	#region Inspector Variables
	[Tooltip("Target the arrow will follow")]
	[SerializeField] public Transform target;
	[Tooltip("How fast the arrow will move towards the goal")]
	[SerializeField] public float strength = 100f;
	[Tooltip("Minimum alpha value")]
	[SerializeField] public float minimum = 0.0f;
	[Tooltip("Maximum alpha value")]
	[SerializeField] public float maximum = 1f;
	[Tooltip("Duration of fade out for the arrow")]
	[SerializeField] public float duration = 7.0f;
	[Tooltip("Sprite to fade out")]
	[SerializeField] public SpriteRenderer sprite;
	#endregion

	#region Private Variables
    [SerializeField]
    private GameDataSO gameData;

	private float startTime;
	#endregion

	#region MonoBehaviour
	void Start ()
	{
		startTime = Time.time;
        StartCoroutine(Blink(duration));
	}

	void LateUpdate () 
	{
		Vector2 direction = target.position - transform.position;
		float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, strength * Time.deltaTime);
        StartCoroutine(FadeOut());
	}
	#endregion

	#region Private Methods
	private IEnumerator FadeOut()
	{
		float t = (Time.time - startTime) / duration;
		sprite.color = new Color(1f,1f,1f,Mathf.SmoothStep(maximum, minimum, t)); 
		yield return null;
	}

    private IEnumerator Blink(float waitTime)
    {
        float endTime = Time.time + waitTime;
        while (Time.time < endTime)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(gameData.blinkMultiplier);
            sprite.enabled = true;
            yield return new WaitForSeconds(gameData.blinkMultiplier);
        }
    }
	#endregion
}
