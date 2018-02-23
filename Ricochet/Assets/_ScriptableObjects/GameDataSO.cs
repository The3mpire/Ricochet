﻿using Enumerables;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Game Data")]
public class GameDataSO : ScriptableObject
{

    [SerializeField]
    [Tooltip("How long the player takes to respawn in seconds")]
    public float playerRespawnTime = 2f;    
    [SerializeField]
    private EMode gameMode;

    [SerializeField]
    private ECharacter[] playerCharacters;
    [SerializeField]
    private ETeam[] playerTeams;
    [SerializeField]
    private bool[] playerActive;

    [SerializeField]
    private BuildIndex gameLevel;
    [Range(1, 100)]
    [SerializeField]
    private int scoreLimit;
    [Tooltip("Default value when game boots up")]
    [Range(1, 100)]
    [SerializeField]
    private int defaultScoreLimit;
    [Range(30, 600)]
    [SerializeField]
    private int timeLimit;
    [Tooltip("Default value when game boots up")]
    [Range(30, 600)]
    [SerializeField]
    private int defaultTimeLimit;

    [Header("AB Test Settings")]
    [Tooltip("Dash Recharge Type")]
    [SerializeField]
    private ERechargeType _rechargeType;
    [Tooltip("Invert Left Trigger?")]
    [SerializeField]
    private ELeftTriggerSetting _leftTriggerSetting;

    private int blueTeamScore, redTeamScore;
    private ETeam gameWinner;
    private Dictionary<string, List<string>> playerKills;
    private Dictionary<string, int> playerDeaths;

    #region Scriptable Object Methods
    public void OnEnable()
    {
        timeLimit = defaultTimeLimit;
        scoreLimit = defaultScoreLimit;
    }
    #endregion

    #region Getters and Setters

    #region Score Limit
    public int GetScoreLimit()
    {
        return scoreLimit;
    }

    public void SetScoreLimit(int value)
    {
        scoreLimit = Mathf.Clamp(value, 1, 100);
    }
    #endregion

    #region Time Limit
    public float GetTimeLimit()
    {
        return timeLimit;
    }

    public void SetTimeLimit(int timeInSeconds)
    {
        timeLimit = Mathf.Clamp(timeInSeconds, 30, 600);
    }
    #endregion

    public BuildIndex GetGameLevel()
    {
        return gameLevel;
    }

    public void SetGameLevel(BuildIndex level)
    {
        gameLevel = level;
    }

    public EMode GetGameMode()
    {
        return gameMode;
    }

    public void SetGameMode(EMode mode)
    {
        gameMode = mode;
    }

    public ECharacter[] GetPlayerCharacters()
    {
        return playerCharacters;
    }

    public void SetPlayerCharacters(ECharacter[] characters)
    {
        playerCharacters = characters;
    }

    public int GetBlueScore()
    {
        return blueTeamScore;
    }

    public void SetBlueScore(int value)
    {
        blueTeamScore = Mathf.Clamp(value, 0, int.MaxValue);
    }

    public int GetRedScore()
    {
        return redTeamScore;
    }

    public void SetRedScore(int value)
    {
        redTeamScore = Mathf.Clamp(value, 0, int.MaxValue);
    }

    public ETeam GetGameWinner()
    {
        return gameWinner;
    }

    public bool GetActive(int playerNumber)
    {
        return playerActive[playerNumber];
    }

    public void SetGameWinner(ETeam winner)
    {
        gameWinner = winner;
    }

    public ERechargeType GetRechargeType()
    {
        return _rechargeType;
    }

    public ELeftTriggerSetting GetLeftTriggerSetting()
    {
        return _leftTriggerSetting;
    }

    #region Player Character
    public ECharacter GetPlayerCharacter(int playerNumber)
    {
        return playerCharacters[playerNumber];
    }

    public void SetPlayerCharacter(int playerNumber, ECharacter eCharacter)
    {
        playerCharacters[playerNumber] = eCharacter;
    }
    #endregion

    #region Player Team
    public ETeam GetPlayerTeam(int playerNumber)
    {
        return playerTeams[playerNumber];
    }

    public void SetPlayerTeam(int playerNumber, ETeam team)
    {
        playerTeams[playerNumber] = team;
    }

    public void SetPlayerActive(int playerNumber)
    {
        playerActive[playerNumber] = true;
    }

    public void SetPlayerInactive(int playerNumber)
    {
        playerActive[playerNumber] = false;
    }
    #endregion

    #endregion

    public void ResetGameStatistics()
    {
        blueTeamScore = 0;
        redTeamScore = 0;
        gameWinner = ETeam.None;
    }

}
