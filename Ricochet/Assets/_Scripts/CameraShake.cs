using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Amount of time the screen will shake for")]
    [SerializeField]
    public float shakeDuration = 0f;
    [Tooltip("Intensity of shake")]
    [SerializeField]
    private float shakeAmount = 0.7f;
    [Tooltip("How fast the shake reduces to its normal state")]
    [SerializeField]
    private float decreaseFactor = 1.0f;
    [Tooltip("The position of the camera(drag the main camera here")]
    [SerializeField]
    private Transform camTransform;
    #endregion

    #region Hidden Variables
    private Vector3 originalPos;
    #endregion

    #region MonoBehavior
    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }
    }
    #endregion
}