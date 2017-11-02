﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;


public class CircleShieldPowerUp : Powerup
{
    [SerializeField]
    private Color shieldColor = Color.blue;
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
                player.ReceivePowerUp(powerUpType);
                player.EnableSecondaryShield(true);
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