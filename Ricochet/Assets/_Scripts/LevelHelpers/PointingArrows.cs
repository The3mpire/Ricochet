using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingArrows : MonoBehaviour 
{

	#region Inspector Variables
    [Tooltip("How frequently the player sprite should blink on death")]
    [SerializeField]public float blinkMultiplier = 0.2f;
	[Tooltip("Target the arrow will follow")]
	[SerializeField] public Transform target;
    [Tooltip("Sprite of the targer that the arrow will follow")]
    [SerializeField] public SpriteRenderer targetSprite;
    [Tooltip("How fast the arrow will move towards the goal")]
	[SerializeField] public float strength = 100f;
	[Tooltip("Minimum alpha value")]
	[SerializeField] public float minimum = 0.0f;
	[Tooltip("Maximum alpha value")]
	[SerializeField] public float maximum = 1f;
	[Tooltip("Duration of fade out for the arrow")]
	[SerializeField] public float duration = 5.0f;
	[Tooltip("Sprite to fade out")]
	[SerializeField] public SpriteRenderer sprite;
	#endregion

	#region Private Variables
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

        //arrows when goals are offscreen
        if (!targetSprite.isVisible)
        {
            sprite.color = new Color(1f, 1f, 1f, 1f);
            sprite.enabled = true;
        }
        else
        {
            //blinking arrows
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, strength * Time.deltaTime);
            StartCoroutine(FadeOut());
        }
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
            yield return new WaitForSeconds(blinkMultiplier);
            sprite.enabled = true;
            yield return new WaitForSeconds(blinkMultiplier);
        }
    }

	#endregion
}
