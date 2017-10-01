using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public Color shieldColor = Color.red;

    #region MonoBehaviour
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (collision.tag == "Player")
        {
            player.shield.color = shieldColor;
            player.hasPowerUp = true;
            GameManager.instance.RespawnPowerUp(this.gameObject);
            gameObject.SetActive(false);
        }
    }
    #endregion
}
