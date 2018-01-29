using Enumerables;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    #region Inspector Variables
    [Header("Multi Ball")]
    [Tooltip("The color of multiball powerup objects")]
    [SerializeField]
    private Color multiBallPowerUpColor;
    [Tooltip("The color of shields with a multiball powerup")]
    [SerializeField]
    private Color multiBallShieldColor = Color.red;

    [Tooltip("The scale of the temporary balls in relation to the original")]
    [Range(0,2)]
    [SerializeField]
    private float tempBallScale = 0.8f;

    [Header("Catch N Throw")]
    [Tooltip("The color of catch n throw powerup objects")]
    [SerializeField]
    private Color catchNThrowPowerUpColor;
    [Tooltip("The color of shields with a catch n throw powerup")]
    [SerializeField]
    private Color catchNThrowShieldColor = Color.red;

    [Header("Circle Shield")]
    [Tooltip("The color of circle shield powerup objects")]
    [SerializeField]
    private Color circleShieldColor;
    [Tooltip("The color of shields with a catch n throw powerup")]
    [SerializeField]
    private Color circleShieldShieldColor = Color.blue;
    #endregion

    #region External Functions
    public Color GetPowerUpColor(EPowerUp ePowerUp)
    {
        switch (ePowerUp)
        {
            case EPowerUp.Multiball:
                return multiBallPowerUpColor;
            case EPowerUp.CatchNThrow:
                return catchNThrowPowerUpColor;
            case EPowerUp.CircleShield:
                return circleShieldColor;
            default:
                return Color.white;
        }
    }

    public Color GetPowerUpShieldColor(EPowerUp ePowerUp)
    {
        switch (ePowerUp)
        {
            case EPowerUp.Multiball:
                return multiBallShieldColor;
            case EPowerUp.CatchNThrow:
                return catchNThrowShieldColor;
            case EPowerUp.CircleShield:
                return circleShieldShieldColor;
            default:
                return Color.white;
        }
    }

    public float GetTempBallScale()
    {
        return tempBallScale;
    }
    #endregion
}