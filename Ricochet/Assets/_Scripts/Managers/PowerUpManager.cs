using Enumerables;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    #region Inspector Variables
    [Header("Powerup Settings")]
    [Tooltip("Shrinks enemy team to this scale")]
    [SerializeField]
    private float shrinkScale = 0.5f;
    [Tooltip("Shrink powerup duration in seconds")]
    [SerializeField]
    private float shrinkDuration = 5f;

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
    [Tooltip("Maximum number of balls allowed")]
    [SerializeField]
    private int maxActiveBalls = 3;
    [Tooltip("Number of balls spawned")]
    [SerializeField]
    private int numBallsSpawned = 2;


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
    public float GetShrinkScale()
    {
        return shrinkScale;
    }
    public float GetShrinkDuration()
    {
        return shrinkDuration;
    }

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

    public int GetMaxBalls()
    {
        return maxActiveBalls;
    }

    public int GetBallSpawnCount()
    {
        return numBallsSpawned;
    }
    #endregion
}