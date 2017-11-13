using Enumerables;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private EPowerUp powerUpType;
    [SerializeField]
    private PowerUpManager powerUpManager;

    private Color powerUpColor;
    private Color shieldColor;
    private GameManager gameManagerInstance;

    void Awake()
    {
        powerUpColor = powerUpManager.GetPowerUpColor(powerUpType);
        shieldColor = powerUpManager.GetPowerUpShieldColor(powerUpType);
    }

    #region MonoBehaviour
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

    public EPowerUp GetPowerUpType()
    {
        return powerUpType;
    }

    public Color GetShieldColor()
    {
        return shieldColor;
    }
}
