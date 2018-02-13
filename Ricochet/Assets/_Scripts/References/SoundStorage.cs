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
    [Tooltip("Drag the cop jetpack sounds here")]
    [SerializeField]
    private List<AudioClip> copJetpackSounds;
    [Tooltip("Drag the cop dash sounds here")]
    [SerializeField]
    private List<AudioClip> copDashSounds;
    [Tooltip("Drag the cop death sounds here")]
    [SerializeField]
    private List<AudioClip> copDeathSounds;
    [Tooltip("Drag the cop taunt sounds here")]
    [SerializeField]
    private List<AudioClip> copTauntSounds;

    [Header("Cat Sounds")]
    [Tooltip("Drag the cat jetpack sounds here")]
    [SerializeField]
    private List<AudioClip> catJetpackSounds;
    [Tooltip("Drag the cat dash sounds here")]
    [SerializeField]
    private List<AudioClip> catDashSounds;
    [Tooltip("Drag the cat death sounds here")]
    [SerializeField]
    private List<AudioClip> catDeathSounds;
    [Tooltip("Drag the computer taunt sounds here")]
    [SerializeField]
    private List<AudioClip> catTauntSounds;

    [Header("Computer Sounds")]
    [Tooltip("Drag the computer jetpack sounds here")]
    [SerializeField]
    private List<AudioClip> computerJetpackSounds;
    [Tooltip("Drag the computer dash sounds here")]
    [SerializeField]
    private List<AudioClip> computerDashSounds;
    [Tooltip("Drag the computer death sounds here")]
    [SerializeField]
    private List<AudioClip> computerDeathSounds;
    [Tooltip("Drag the computer taunt sounds here")]
    [SerializeField]
    private List<AudioClip> computerTauntSounds;

    [Space]

    [Header("Miscellaneous Sounds")]
    [Tooltip("Drag menu click sounds here")]
    [SerializeField]
    private List<AudioClip> menuSounds;
    [Tooltip("Drag ball sounds here")]
    [SerializeField]
    private List<AudioClip> ballSounds;
    [Tooltip("Drag scoring sounds here")]
    [SerializeField]
    private List<AudioClip> scoringSounds;
    #endregion

    #region Player Sounds
    public AudioClip GetPlayerDeathSound(ECharacter character)
    {
        switch (character)
        {
            case ECharacter.CatManP:
            case ECharacter.CatManWT:
                return catDeathSounds[Random.Range(0, catDeathSounds.Count)];
            case ECharacter.Computer:
                return computerDeathSounds[Random.Range(0, computerDeathSounds.Count)];
            default: // mallCop
                return copDeathSounds[Random.Range(0, copDeathSounds.Count)];
        }
    }

    public AudioClip GetPlayerDashSound(ECharacter character)
    {
        switch (character)
        {
            case ECharacter.CatManP:
            case ECharacter.CatManWT:
                return catDashSounds[Random.Range(0, catDashSounds.Count)];
            case ECharacter.Computer:
                return computerDashSounds[Random.Range(0, computerDashSounds.Count)];
            default: // mallCop
                return copDashSounds[Random.Range(0, copDashSounds.Count)];
        }
    }
    public AudioClip GetPlayerTauntSound(ECharacter character)
    {
        switch (character)
        {
            case ECharacter.CatManP:
            case ECharacter.CatManWT:
            case ECharacter.Computer:
            default: // for now we only have one asset, and its super standin, i mean quality
                return computerTauntSounds[Random.Range(0, computerTauntSounds.Count)];
        }
    }
    public AudioClip GetPlayerJetpackSound(ECharacter character)
    {
        switch (character)
        {
            case ECharacter.CatManP:
            case ECharacter.CatManWT:
                return catJetpackSounds[Random.Range(0, catJetpackSounds.Count)];
            case ECharacter.Computer:
                return computerJetpackSounds[Random.Range(0, computerJetpackSounds.Count)];
            default: // mallCop
                return copJetpackSounds[Random.Range(0, copJetpackSounds.Count)];
        }
    }
    #endregion

    #region Miscellaneous Sounds
    public AudioClip GetMenuClickSound()
    {
        return menuSounds[Random.Range(0, menuSounds.Count)];
    }

    public AudioClip GetScoringSound()
    {
        return scoringSounds[Random.Range(0, scoringSounds.Count)];
    }

    public AudioClip GetBallSound()
    {
        return ballSounds[Random.Range(0, ballSounds.Count)];
    }
    #endregion
}