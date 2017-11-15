using Enumerables;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Which type of powerup this is")]
    [SerializeField]
    private EPowerUp powerUpType;
    #endregion

    #region Hidden Variables
    private GameManager gameManagerInstance;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            gameObject.GetComponent<SpriteRenderer>().color = gameManagerInstance.GetPowerUpColor(powerUpType);
        }
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
    #endregion
}
