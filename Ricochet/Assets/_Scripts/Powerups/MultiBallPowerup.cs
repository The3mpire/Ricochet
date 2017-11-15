using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;


public class MultiBallPowerup : Powerup
{
    [SerializeField]
    private Color shieldColor = Color.red;
    [SerializeField]
    private EPowerUp powerUpType;

    private GameManager gameManagerInstance;

    #region MonoBehaviour
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
            {
                PlayerController player = collision.GetComponent<PlayerController>();

                player.GetShield().SetColor(shieldColor);
                player.ReceivePowerUp(powerUpType);
                gameManagerInstance.RespawnPowerUp(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
    #endregion 

    protected override EPowerUp PowerUpType
    {
        get
        {
            return powerUpType;
        }

    }

    protected override Color ShieldColor
    {
        get
        {
            return shieldColor;
        }
    }

}
