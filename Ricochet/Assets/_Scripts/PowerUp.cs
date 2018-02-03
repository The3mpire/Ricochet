﻿using Enumerables;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class PowerUp : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField]
    private GameDataSO gameData;
    [Tooltip("Which type of powerup this is")]
    [SerializeField] private EPowerUp powerUpType;
    [Tooltip("The sprite of the powerup")]
    [SerializeField] private SpriteRenderer powerupSprite;
    #endregion

    #region Hidden Variables
    private GameManager gameManagerInstance;
    private System.Random rng;
    private List<EPowerUp> powerups;
    private EPowerUp instanceType;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        powerupSprite = this.GetComponent<SpriteRenderer>();
        if (powerUpType == EPowerUp.Random)
        {
            rng = new System.Random();
            powerups = Enum.GetValues(typeof(EPowerUp)).Cast<EPowerUp>().ToList();
            if (gameData.GetGameMode() == EMode.Deathmatch)
            {
                powerups = powerups.Where(p => (p != EPowerUp.Random) && (p != EPowerUp.None)).ToList();
            }
            else
            {
                powerups = powerups.Where(p => (p != EPowerUp.Random) && (p != EPowerUp.None) && (p != EPowerUp.Multiball)).ToList();
            }
        }
        else
        {
            instanceType = powerUpType;
            UpdateSprite();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
            {
                gameManagerInstance.PlayerPowerUpCollision(collider.gameObject, this);
                gameObject.SetActive(false);
                gameManagerInstance.RespawnPowerUp(gameObject);
            }
        }
    }

    void OnEnable()
    {
        if (powerUpType == EPowerUp.Random)
        {
            instanceType = powerups[rng.Next(powerups.Count)];
            UpdateSprite();
        }
    }
    #endregion

    #region External Functions
    public EPowerUp GetPowerUpType()
    {
        return instanceType;
    }
    #endregion
    
    #region Private Fucntions
    private void UpdateSprite()
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            switch (instanceType)
            {
                case EPowerUp.Multiball:
                    powerupSprite.sprite = Resources.Load<Sprite>("_Art/2D Sprites/Environment/Powerups/multiballiconplaceholder");
                    break;
                case EPowerUp.CatchNThrow:
                    powerupSprite.sprite = Resources.Load<Sprite>("_Art/2D Sprites/Environment/Powerups/catchstickyiconplaceholder");
                    break;
                case EPowerUp.CircleShield:
                    powerupSprite.sprite = Resources.Load<Sprite>("_Art/2D Sprites/Environment/Powerups/fullshieldiconplaceholder");
                    break;
                case EPowerUp.Freeze:
                    powerupSprite.sprite = Resources.Load<Sprite>("_Art/2D Sprites/Environment/Powerups/FreezieWiiU");
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
}
