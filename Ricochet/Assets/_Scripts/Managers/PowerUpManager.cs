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
    private Color multiBallShieldColor;
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
    private Color catchNThrowShieldColor;

    [Header("Circle Shield")]
    [Tooltip("The color of circle shield powerup objects")]
    [SerializeField]
    private Color circleShieldColor;
    [Tooltip("The color of the full shield")]
    [SerializeField]
    private Color circleShieldShieldColor = Color.blue;
    [Tooltip("Burst force when shield ends")]
    [SerializeField]
    private float burstForce = 100f;
    [Tooltip("Radius of the burst when shield ends")]
    [SerializeField]
    private float burstRadius = 8f;

    [Header("Freeze")]
    [Tooltip("The color of freeze powerup objects")]
    [SerializeField]
    private Color freezeColor;
    [Tooltip("The color of shield with freeze power up")]
    [SerializeField]
    private Color freezeShieldColor;
    [Tooltip("The amount of time a character stays frozen")]
    [SerializeField]
    private float freezeTime;

    
    #endregion

    #region External Functions
    public float GetBurstRadius()
    {
        return burstRadius;
    }

    public float GetBurstForce()
    {
        return burstForce;
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
            case EPowerUp.Freeze:
                return freezeColor;
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
            case EPowerUp.Freeze:
                return freezeShieldColor;
            default:
                return Color.white;
        }
    }

    public float GetFreezeTime()
    {
        return freezeTime;
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