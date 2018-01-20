using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Amount of time the screen will shake for")]
    [SerializeField]
    public float shakeDuration = 2f;
    [Tooltip("Intensity of shake")]
    [SerializeField]
    private float shakeAmount = 0.7f;
    [Tooltip("How fast the shake reduces to its normal state")]
    [SerializeField]
    private float decreaseFactor = 1.0f;
    [Tooltip("Drag the main camera here")]
    [SerializeField]
    private Transform camTransform;
    #endregion

    #region Hidden Variables
    private Vector3 originalPos;
    private float duration;
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
    #endregion

    #region Public Methods
    public void Shake()
    {
        duration = shakeDuration;
        
        if (duration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            duration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            duration = 0f;
            camTransform.localPosition = originalPos;
        }
    }
    #endregion
}