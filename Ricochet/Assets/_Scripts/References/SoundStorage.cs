using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

[CreateAssetMenu(fileName = "SoundStorage", menuName = "Sound", order = 1)]

public class SoundStorage : ScriptableObject
{
    #region Reference Variables
    [Header("Player Sounds")]
    [Space]
    [Header("Cop Sounds")]
    [SerializeField]
    private List<AudioClip> copJetpackSounds;
    [SerializeField]
    private List<AudioClip> copDashSounds;
    [SerializeField]
    private List<AudioClip> copDeathSounds;
    [SerializeField]
    private List<AudioClip> copTauntSounds;
    [SerializeField]
    private List<AudioClip> copBumpSounds;
    [SerializeField]
    private List<AudioClip> copRespawnSounds;

    [Header("Cat Sounds")]
    [SerializeField]
    private List<AudioClip> catJetpackSounds;
    [SerializeField]
    private List<AudioClip> catDashSounds;
    [SerializeField]
    private List<AudioClip> catDeathSounds;
    [SerializeField]
    private List<AudioClip> catTauntSounds;
    [SerializeField]
    private List<AudioClip> catBumpSounds;
    [SerializeField]
    private List<AudioClip> catRespawnSounds;

    [Header("Computer Sounds")]
    [SerializeField]
    private List<AudioClip> computerJetpackSounds;
    [SerializeField]
    private List<AudioClip> computerDashSounds;
    [SerializeField]
    private List<AudioClip> computerDeathSounds;
    [SerializeField]
    private List<AudioClip> computerTauntSounds;
    [SerializeField]
    private List<AudioClip> computerBumpSounds;
    [SerializeField]
    private List<AudioClip> computerRespawnSounds;

    [Header("Fish Sounds")]
    [SerializeField]
    private List<AudioClip> fishJetpackSounds;
    [SerializeField]
    private List<AudioClip> fishDashSounds;
    [SerializeField]
    private List<AudioClip> fishDeathSounds;
    [SerializeField]
    private List<AudioClip> fishTauntSounds;
    [SerializeField]
    private List<AudioClip> fishBumpSounds;
    [SerializeField]
    private List<AudioClip> fishRespawnSounds;

    [Space]

    [Header("Powerup Sounds")]
    [Space]
    [Header("Multiball Sounds")]
    [SerializeField]
    private AudioClip multiballPickedUp;
    [SerializeField]
    private AudioClip multiballUsed;

    [Header("Freeze Sounds")]
    [SerializeField]
    private AudioClip freezePickedUp;
    [SerializeField]
    private AudioClip freezeUsed;

    [Header("CatchNThrow")]
    [SerializeField]
    private AudioClip catchNThrowPickedUp;
    [SerializeField]
    private AudioClip catchNThrowUsed;

    [Header("CircleShield")]
    [SerializeField]
    private AudioClip circleShieldPickedup;
    [SerializeField]
    private AudioClip circleShieldUsed;

    [Header("Shrink")]
    [SerializeField]
    private AudioClip shrinkPickedUp;
    [SerializeField]
    private AudioClip shrinkUsed;

    [Space]

    [Header("Ball Sounds")]
    [SerializeField]
    private List<AudioClip> ballWallBumpSounds;
    [SerializeField]
    private List<AudioClip> ballShieldHitSounds;
    [SerializeField]
    private List<AudioClip> ballFastShieldHitSounds;
    [SerializeField]
    private List<AudioClip> ballGoalSounds;

    [Space]
    
    [Header("Menu Sounds")]
    [SerializeField]
    private AudioClip menuTraversalSound;
    [SerializeField]
    private AudioClip menuConfirmSound;
    [SerializeField]
    private AudioClip menuBackSound;
    [SerializeField]
    private AudioClip menuPauseSound;
    [SerializeField]
    private AudioClip menuUnpauseSound;

    [Space]

    [Header("Miscellaneous Sounds")]
    [SerializeField]
    private AudioClip initialSpawnSound;
    [SerializeField]
    private AudioClip countdownTimerSound;
    [SerializeField]
    private AudioClip matchBeginSound;
    [SerializeField]
    private AudioClip matchEndSound;
    [SerializeField]
    private AudioClip redTeamWinSound;
    [SerializeField]
    private AudioClip blueTeamWinSound;
    [SerializeField]
    private AudioClip sceneTransitionSound;
    #endregion

    #region Player Sounds
    public AudioClip GetPlayerDeathSound(ECharacter character)
    {
        switch (character)
        {
            case ECharacter.Cat:
                return catDeathSounds[Random.Range(0, catDeathSounds.Count)];
            case ECharacter.Computer:
                return computerDeathSounds[Random.Range(0, computerDeathSounds.Count)];
            case ECharacter.Sushi:
                return fishDeathSounds[Random.Range(0, fishDeathSounds.Count)];
            default: // mallCop
                return copDeathSounds[Random.Range(0, copDeathSounds.Count)];
        }
    }

    public AudioClip GetPlayerDashSound(ECharacter character)
    {
        switch (character)
        {
            case ECharacter.Cat:
                return catDashSounds[Random.Range(0, catDashSounds.Count)];
            case ECharacter.Computer:
                return computerDashSounds[Random.Range(0, computerDashSounds.Count)];
            case ECharacter.Sushi:
                return fishDashSounds[Random.Range(0, fishDashSounds.Count)];
            default: // mallCop
                return copDashSounds[Random.Range(0, copDashSounds.Count)];
        }
    }
    public AudioClip GetPlayerTauntSound(ECharacter character)
    {
        switch (character)
        {
            case ECharacter.Cat:
            case ECharacter.Computer:
            default: // for now we only have one asset, and its super standin, i mean quality
                return computerTauntSounds[Random.Range(0, computerTauntSounds.Count)];
        }
    }
    public AudioClip GetPlayerJetpackSound(ECharacter character)
    {
        switch (character)
        {
            case ECharacter.Cat:
                return catJetpackSounds[Random.Range(0, catJetpackSounds.Count)];
            case ECharacter.Computer:
                return computerJetpackSounds[Random.Range(0, computerJetpackSounds.Count)];
            case ECharacter.Sushi:
                return fishJetpackSounds[Random.Range(0, fishJetpackSounds.Count)];
            default: // mallCop
                return copJetpackSounds[Random.Range(0, copJetpackSounds.Count)];
        }
    }

    public AudioClip GetPlayerBumpSound(ECharacter character)
    {
        switch(character)
        {
            case ECharacter.Cat:
                return catBumpSounds[Random.Range(0, catBumpSounds.Count)];
            case ECharacter.Computer:
                return computerBumpSounds[Random.Range(0, computerBumpSounds.Count)];
            case ECharacter.Sushi:
                return fishBumpSounds[Random.Range(0, fishBumpSounds.Count)];
            default: // mallCop
                return copBumpSounds[Random.Range(0, copBumpSounds.Count)];
        }
    }

    public AudioClip GetPlayerRespawnSound(ECharacter character)
    {
        switch (character)
        {
            case ECharacter.Cat:
                return catRespawnSounds[Random.Range(0, catRespawnSounds.Count)];
            case ECharacter.Computer:
                return computerRespawnSounds[Random.Range(0, computerRespawnSounds.Count)];
            case ECharacter.Sushi:
                return fishRespawnSounds[Random.Range(0, fishRespawnSounds.Count)];
            default: // mallCop
                return copRespawnSounds[Random.Range(0, copRespawnSounds.Count)];
        }
    }
    #endregion

    #region Menu Sounds
    public AudioClip GetMenuTraverseSounds()
    {
        return menuTraversalSound;
    }

    public AudioClip GetMenuClickSound()
    {
        return menuConfirmSound;
    }

    public AudioClip GetMenuBackSound()
    {
        return menuBackSound;
    }

    public AudioClip GetPauseSound()
    {
        return menuPauseSound;
    }

    public AudioClip GetUnpauseSound()
    {
        return menuUnpauseSound;
    }
    #endregion

    #region Ball Sounds
    public AudioClip GetScoringSound()
    {
        return ballGoalSounds[Random.Range(0, ballGoalSounds.Count)];
    }

    public AudioClip GetBallSound(string tag)
    {
        switch (tag)
        {
            case "Wall":
                return ballWallBumpSounds[Random.Range(0, ballWallBumpSounds.Count)];
            default: // "Shield":
                return ballShieldHitSounds[Random.Range(0, ballShieldHitSounds.Count)];
        }
    }
    #endregion

    #region Powerup Sounds
    public AudioClip GetPowerUpPickUpSound(EPowerUp powerup)
    {
        switch(powerup)
        {
            case EPowerUp.CatchNThrow:
                return catchNThrowPickedUp;
            case EPowerUp.CircleShield:
                return circleShieldPickedup;
            case EPowerUp.Freeze:
                return freezePickedUp;
            case EPowerUp.Multiball:
                return multiballPickedUp;
            default: // EPowerUp.Shrink:
                return shrinkPickedUp;
        }
    }

    public AudioClip GetPowerUpUsedSound(EPowerUp powerUp)
    {
        switch(powerUp)
        {
            case EPowerUp.CatchNThrow:
                return catchNThrowUsed;
            case EPowerUp.CircleShield:
                return circleShieldUsed;
            case EPowerUp.Freeze:
                return freezeUsed;
            case EPowerUp.Multiball:
                return freezeUsed;
            default: //EPowerUp.Shrink
                return shrinkUsed;
        }
    }
    #endregion
}