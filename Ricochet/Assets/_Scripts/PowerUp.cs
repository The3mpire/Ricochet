using Enumerables;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class PowerUp : MonoBehaviour
{
    #region Inspector Variables
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
            switch (powerUpType)
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
                default:
                    break;
            }
        }
    }
    #endregion
}
