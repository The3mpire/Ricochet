using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Amount of time the screen will shake for")]
    [SerializeField]
    public float shakeDuration = 2f;
    [Tooltip("Intensity of shake")]
    [SerializeField]
    private float shakeStrength = 1.0f;
    [Tooltip("How much the shake vibrates")]
    [SerializeField]
    private int shakeVibrato = 10;
    [Tooltip("How random the shake directions are")]
    [SerializeField]
    [Range(0, 90)]
    private int shakeRandomness = 90;
    #endregion

    #region Public Methods
    public void Shake()
    {
        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness);
    }
    #endregion
}