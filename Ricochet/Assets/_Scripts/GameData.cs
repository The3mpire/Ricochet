using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine.Experimental.Rendering;
using UnityEngine;
using Enumerables;

public static class GameData {
    #region Private
    #region Game Setup
    private static int _matchScoreLimit;
    private static float _matchTimeLimit;
    private static int _playerCount;
    private static EMode _gameMode;
    private static Enumerables.ECharacter[] _playerCharacters;
    private static Enumerables.ETeam[] _playerTeams;
	private static bool[] playersActive = {true, true, false, false};
    #endregion

    #region Game Statistics
    private static int _blueTeamScore, _redTeamScore;
    private static Enumerables.ETeam _gameWinner;
    private static Dictionary<string, List<string>> _playerKills;
    private static Dictionary<string, int> _playerDeaths;
    #endregion
    #endregion

    #region Getters and Setters
    public static BuildIndex GameLevel { get; set; }

    public static int matchScoreLimit
    {
        get
        {
            return _matchScoreLimit;
        }
        set
        {
            if (value < 0)
            {
                _matchScoreLimit = 10;
            }
            else if (value > 100)
            {
                _matchScoreLimit = 100;
            }
            else
            {
                _matchScoreLimit = value;
            }
        }
    }
    public static float matchTimeLimit
    {
        get
        {
            return _matchTimeLimit;
        }
        set
        {
            if (value < 0)
            {
                _matchTimeLimit = 10;
            }
            else if (value > 100)
            {
                _matchTimeLimit = 100;
            }
            else
            {
                _matchTimeLimit = value;
            }
        }
    }
    public static int playerCount
    {
        get
        {
            return _playerCount;
        }
        set
        {
            if (value < 2)
            {
                _playerCount = 2;
            }
            else if (value > 8)
            {
                _playerCount = 8;
            }
            else
            {
                _playerCount = value;
            }
        }
    }
    public static EMode gameMode
    {
        get
        {
            return _gameMode;
        }
        set
        {
            _gameMode = value;
        }
    }

    public static Enumerables.ECharacter[] playerCharacters
    {
        get { return _playerCharacters; }
        set { _playerCharacters = value; }
    }

    public static Enumerables.ETeam[] playerTeams
    {
        get { return _playerTeams; }
        set { _playerTeams = value; }
    }
    public static int blueTeamScore
    {
        get
        {
            return _blueTeamScore;
        }
        set
        {
            if (value < 0)
            {
                _blueTeamScore = 0;
            }
            else
            {
                _blueTeamScore = value;
            }
        }
    }
    public static int redTeamScore
    {
        get
        {
            return _redTeamScore;
        }
        set
        {
            if (value < 0)
            {
                _redTeamScore = 0;
            }
            else
            {
                _redTeamScore = value;
            }
        }
    }
    public static Enumerables.ETeam gameWinner
    {
        get { return _gameWinner; }
        set { _gameWinner = value; }
    }
    public static Dictionary<string, List<string>> playerKills
    {
        get
        {
            return _playerKills;
        }
        set
        {
            _playerKills = value;
        }
    }
    public static Dictionary<string, int> playerDeaths
    {
        get
        {
            return _playerDeaths;
        }
        set
        {
            _playerDeaths = value;
        }
    }
	public static void SetPlayerActive(int playerNumber, bool value)
	{
		if (playerNumber > playersActive.Length) {
			Debug.LogError ("Player " + playerNumber + " is not within playersActive (in GameData.cs)");
			return;
		}
		playersActive [playerNumber - 1] = value;
	}
	public static bool PlayerIsActive(int playerNumber)
	{
		if (playerNumber > playersActive.Length) {
			Debug.LogError ("Player " + playerNumber + " is not within playersActive (in GameData.cs)");
			return false;
		}
		return playersActive [playerNumber - 1];
	}
	public static void ResetPlayerActive(int numberOfPlayers)
	{
		playersActive = new bool[numberOfPlayers];
	}
    #endregion

    #region Functions
    public static void ResetGameSetup()
    {
        _matchScoreLimit = 0;
        _matchTimeLimit = 0;
        _playerCount = 0;
        _playerTeams = null;
        _playerCharacters = null;
    }
    /// <summary>
    /// Resets all variables in Game Statistics region
    /// </summary>
    public static void ResetGameStatistics()
    {
        _blueTeamScore = 0;
        _redTeamScore = 0;
        _gameWinner = Enumerables.ETeam.None;
        _playerKills = null;
        _playerDeaths = null;
    }
    #endregion
}
