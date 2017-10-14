using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;


public class PowerUp : MonoBehaviour
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

                player.GetShieldSpriteRenderer().color = shieldColor;
                gameManagerInstance.RespawnPowerUp(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
    #endregion 
}
