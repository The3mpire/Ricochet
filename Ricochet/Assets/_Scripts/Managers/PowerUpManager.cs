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
    [SerializeField]
    private RuntimeAnimatorController multiBallAC;
    [SerializeField]
    private Sprite multiBallSprite;


    [Header("Catch N Throw")]
    [Tooltip("The color of catch n throw powerup objects")]
    [SerializeField]
    private Color catchNThrowPowerUpColor;
    [Tooltip("The color of shields with a catch n throw powerup")]
    [SerializeField]
    private Color catchNThrowShieldColor;
    [SerializeField]
    private RuntimeAnimatorController catchNThrowAC;
    [SerializeField]
    private Sprite catchNThrowSprite;

    [Header("Circle Shield")]
    [Tooltip("The color of circle shield powerup objects")]
    [SerializeField]
    private Color circleShieldColor;
    [Tooltip("The color of the full shield")]
    [SerializeField]
    private Color circleShieldShieldColor = new Color32(0, 0, 255, 255);
    [Tooltip("Burst force when shield ends")]
    [SerializeField]
    private float burstForce = 100f;
    [Tooltip("Radius of the burst when shield ends")]
    [SerializeField]
    private float burstRadius = 8f;
    [SerializeField]
    private RuntimeAnimatorController circleShieldAC;
    [SerializeField]
    private Sprite circleShieldSprite;

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
    [SerializeField]
    private RuntimeAnimatorController freezeAC;
    [SerializeField]
    private Sprite freezeSprite;

    [Header("Shrink")]
    [Tooltip("The color of shrink powerup objects")]
    [SerializeField]
    private Color shrinkColor;
    [Tooltip("The color of shield with shrink power up")]
    [SerializeField]
    private Color shrinkShieldColor;
    [Tooltip("Shrinks enemy team to this scale")]
    [SerializeField]
    private float shrinkScale = 0.5f;
    [Tooltip("Shrink powerup duration in seconds")]
    [SerializeField]
    private float shrinkDuration = 5f;
    [Tooltip("Mass of shrunken players multiplier of starting mass")]
    [SerializeField]
    private float shrinkMass = .2f;
    [Tooltip("Multiplier for shrunken move speed")]
    [SerializeField]
    private float shrinkSpeed = .4f;
    [Tooltip("Seconds of invulnerability given to players when they expand to prevent colliding with walls")]
    [SerializeField]
    private float shrinkIFrames = .001f;
    [SerializeField]
    private RuntimeAnimatorController shrinkAC;
    [SerializeField]
    private Sprite shrinkSprite;
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
    public float GetShrinkMass()
    {
        return shrinkMass;
    }
    public float GetShrinkSpeed()
    {
        return shrinkSpeed;
    }
    public float GetIFrames()
    {
        return shrinkIFrames;
    }


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
            case EPowerUp.Shrink:
                return shrinkColor;
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
            case EPowerUp.Shrink:
                return shrinkShieldColor;
            default:
                return Color.white;
        }
    }

    public RuntimeAnimatorController GetPowerUpAC(EPowerUp ePowerUp)
    {
        switch(ePowerUp)
        {
            case EPowerUp.Multiball:
                return multiBallAC;
            case EPowerUp.CatchNThrow:
                return catchNThrowAC;
            case EPowerUp.CircleShield:
                return circleShieldAC;
            case EPowerUp.Freeze:
                return freezeAC;
            case EPowerUp.Shrink:
                return shrinkAC;
            default:
                return null;
        }
    }

    public Sprite GetPowerUpSprite(EPowerUp ePowerUp)
    {
        switch(ePowerUp)
        {
            case EPowerUp.Multiball:
                return multiBallSprite;
            case EPowerUp.CatchNThrow:
                return catchNThrowSprite;
            case EPowerUp.CircleShield:
                return circleShieldSprite;
            case EPowerUp.Freeze:
                return freezeSprite;
            case EPowerUp.Shrink:
                return shrinkSprite;
            default:
                return null;
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