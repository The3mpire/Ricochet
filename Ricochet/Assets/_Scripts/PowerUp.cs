using Enumerables;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class PowerUp : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Which type of powerup this is")]
    [SerializeField]
    private EPowerUp powerUpType;
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
        if (powerUpType == EPowerUp.Random)
        {
            rng = new System.Random();
            powerups = Enum.GetValues(typeof(EPowerUp)).Cast<EPowerUp>().ToList();
            powerups = powerups.Where(p => (p != EPowerUp.Random) && (p != EPowerUp.None)).ToList();
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
                gameManagerInstance.RespawnPowerUp(gameObject);
                gameObject.SetActive(false);
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
            gameObject.GetComponent<SpriteRenderer>().color = gameManagerInstance.GetPowerUpColor(instanceType);
        }
    }
    #endregion
}
