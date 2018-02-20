using Enumerables;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class PowerUp : MonoBehaviour
{
    #region Inspector Variables
    [Header("other")]
    [SerializeField]
    private GameDataSO gameData;
    [Tooltip("Which type of powerup this is")]
    [SerializeField] private EPowerUp powerUpType;
    [Tooltip("The sprite of the powerup")]
    [SerializeField] private SpriteRenderer powerupSprite;
    [Tooltip("The animation controller of the powerup")]
    [SerializeField]
    private Animator powerupAnimator;

    [Serializable]
    private struct weight
    {
        public EPowerUp type;
        [Tooltip("Weighted against other probabilities (not a float)")]
        public uint probability;
    }
    [Header("Powerup spawning weights")]
    [Tooltip("When enabled runs a 1000 trials and prints expirimental probabilities for each powerup")]
    [SerializeField]
    private bool runTrials = false;
    [SerializeField]
    [Tooltip("Only used when type is random")]
    private weight[] weights;
    #endregion

    #region Hidden Variables
    private GameManager gameManagerInstance;
    private System.Random rng;
    private EPowerUp[] powerups;
    private EPowerUp instanceType;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        powerupSprite = this.GetComponent<SpriteRenderer>();
        powerupAnimator = this.GetComponent<Animator>();
        if (powerUpType == EPowerUp.Random)
        {
            uint len = 0;
            rng = new System.Random();
            foreach (weight w in weights)
            {
                if (w.type == EPowerUp.Random || w.type == EPowerUp.None)
                {
                    Debug.LogError("Powerup weight type of " + w.type.ToString(), gameObject);
                    continue;
                }

                len += w.probability;
            }
            powerups = new EPowerUp[len];
            foreach (weight w in weights)
            {
                if (w.type == EPowerUp.Random || w.type == EPowerUp.None)
                {
                    continue;
                }

                for (int i = 0; i < w.probability; i++)
                {
                    powerups[--len] = w.type;
                }
            }
            if (runTrials)
            {
                Trial();
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
        if (collider.CompareTag("Player") && collider.gameObject.layer == LayerMask.NameToLayer("Player"))
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
            instanceType = powerups[rng.Next(0,powerups.Length)];
            UpdateSprite();
        }
    }

    // helps give exp
    private void Trial()
    {
        Dictionary<EPowerUp, float> results = new Dictionary<EPowerUp, float>();
        float trials = 1000;
        foreach (weight w in weights)
        {
            if (results.ContainsKey(w.type))
            {
                Debug.LogError("Duplicate powerup type entries in weights", gameObject);
                continue;
            }
            results.Add(w.type, 0);
        }
        for (float i = 0; i < trials; i++)
        {
            int index = rng.Next(0, powerups.Length);
            results[powerups[index]] = results[powerups[index]] + 1;
        }

        foreach (weight w in weights)
        {
            Debug.Log(w.type.ToString() + ": " + (results[w.type] / trials * 100).ToString() + '%');
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
            AssetBundle powerUpBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "AssetBundles/powerup"));
            UnityEngine.Object powerUpAssets = null;
            switch (instanceType)
            {
                case EPowerUp.Multiball:
                    powerUpAssets = powerUpBundle.LoadAsset("multi_ball_0", typeof(RuntimeAnimatorController));
                    powerupAnimator.runtimeAnimatorController = powerUpAssets as RuntimeAnimatorController;
                    break;
                case EPowerUp.CatchNThrow:
                    powerUpAssets = powerUpBundle.LoadAsset("CatchNThrow_0", typeof(RuntimeAnimatorController));
                    powerupAnimator.runtimeAnimatorController = powerUpAssets as RuntimeAnimatorController;
                    break;
                case EPowerUp.CircleShield:
                    powerUpAssets = powerUpBundle.LoadAsset("360_shield_0", typeof(RuntimeAnimatorController));
                    powerupAnimator.runtimeAnimatorController = powerUpAssets as RuntimeAnimatorController;
                    break;
                case EPowerUp.Freeze:
                    powerUpAssets = powerUpBundle.LoadAsset("freeze_icon_0", typeof(RuntimeAnimatorController));
                    powerupAnimator.runtimeAnimatorController = powerUpAssets as RuntimeAnimatorController;
                    break;
                case EPowerUp.Shrink:
                    powerUpAssets = powerUpBundle.LoadAsset("shrink", typeof(RuntimeAnimatorController));
                    powerupAnimator.runtimeAnimatorController = powerUpAssets as RuntimeAnimatorController;
                    break;
                default:
                    break;
            }
            powerUpBundle.Unload(false);
        }
    }
    #endregion
}
