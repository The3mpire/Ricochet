using UnityEngine;
using System.Collections;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Array of all the backgrounds to be parallaxed.")]
    private BackgroundItem[] backgrounds;				

    [SerializeField]
    [Tooltip("The proportion of the camera's movement to move the backgrounds by.")]
    private float parallaxScale;					

    [SerializeField]
    [Tooltip("How much less each successive layer should parallax.")]
    private float parallaxReductionFactor;		

    [SerializeField]
    [Tooltip("How smooth the parallax effect should be")]
    private float smoothing;


	private Transform cam;						// Shorter reference to the main camera's transform.
	private Vector3 previousCamPos;				// The postion of the camera in the previous frame.


	void Awake ()
	{
		cam = Camera.main.transform;
	}


	void Start ()
	{
		previousCamPos = cam.position;
	}


	void Update ()
	{
        // The parallax is the opposite of the camera movement since the previous frame multiplied by the scale.
        float parallax = 0f;
        // For each successive background...
        for (int i = 0; i < backgrounds.Length; i++)
		{
            if (backgrounds[i].useCustomScale)
            {
                parallax = (previousCamPos.x - cam.position.x) * backgrounds[i].customScale;
            } else
            {
                parallax = (previousCamPos.x - cam.position.x) * parallaxScale;
            }

			// ... set a target x position which is their current position plus the parallax multiplied by the reduction.
			float backgroundTargetPosX = backgrounds[i].background.position.x + parallax * (i * parallaxReductionFactor + 1);

			// Create a target position which is the background's current position but with it's target x position.
			Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].background.position.y, backgrounds[i].background.position.z);

			// Lerp the background's position between itself and it's target position.
			backgrounds[i].background.position = Vector3.Lerp(backgrounds[i].background.position, backgroundTargetPos, smoothing * Time.deltaTime);
		}

        // Set the previousCamPos to the camera's position at the end of this frame.
		previousCamPos = cam.position;
	}

    [System.Serializable]
    struct BackgroundItem
    {
        public Transform background;
        public bool useCustomScale;
        public float customScale;
    }
}
