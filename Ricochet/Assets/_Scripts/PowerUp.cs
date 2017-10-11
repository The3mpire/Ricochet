using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;


public class PowerUp : MonoBehaviour
{
    public Color shieldColor = Color.red;
    public EPowerUp powerUpType;

    #region MonoBehaviour
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            player.shield.color = shieldColor;
           // player.hasPowerUp = true;
            GameManager.instance.RespawnPowerUp(this.gameObject);
            gameObject.SetActive(false);
        }
    }
    #endregion 
}
