using Enumerables;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Which type of powerup this is")]
    [SerializeField]
    private EPowerUp powerUpType;
    [Tooltip("Drag the PowerUp Manager here")]
    [SerializeField]
    private PowerUpManager powerUpManager;
    #endregion

    #region Hidden Variables
    private Color powerUpColor;
    private Color shieldColor;
    private GameManager gameManagerInstance;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        powerUpColor = powerUpManager.GetPowerUpColor(powerUpType);
        shieldColor = powerUpManager.GetPowerUpShieldColor(powerUpType);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
            {
                gameManagerInstance.PlayerPowerUpCollision(collider.gameObject, this);
                gameManagerInstance.RespawnPowerUp(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region External Functions
    public EPowerUp GetPowerUpType()
    {
        return powerUpType;
    }

    public Color GetShieldColor()
    {
        return shieldColor;
    }
    #endregion
}
