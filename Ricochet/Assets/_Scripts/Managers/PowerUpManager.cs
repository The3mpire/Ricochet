using Enumerables;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    #region Inspector Variables
    [Header("Multi Ball")]
    [SerializeField]
    private Color multiBallPowerUpColor;
    [SerializeField]
    private Color multiBallShieldColor = Color.black;

    [Header("Catch N Throw")]
    [SerializeField]
    private Color catchNThrowPowerUpColor;
    [SerializeField]
    private Color catchNThrowShieldColor = Color.red;
    #endregion

    public Color GetPowerUpColor(EPowerUp ePowerUp)
    {
        Color c = Color.white;
        switch (ePowerUp)
        {
            case EPowerUp.Multiball:
                c = multiBallPowerUpColor;
                break;
            case EPowerUp.CatchNThrow:
                c = catchNThrowPowerUpColor;
                break;
        }
        return c;
    }

    public Color GetPowerUpShieldColor(EPowerUp ePowerUp)
    {
        Color c = Color.white;
        switch (ePowerUp)
        {
            case EPowerUp.Multiball:
                c = multiBallShieldColor;
                break;
            case EPowerUp.CatchNThrow:
                c = catchNThrowShieldColor;
                break;
        }
        return c;
    }
}
