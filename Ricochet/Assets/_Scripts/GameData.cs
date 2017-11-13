using System.Collections;
using System.Collections.Generic;

public static class GameData {
    #region Private
    #region Game Setup
    private static int _matchScoreLimit;
    private static int _matchTimeLimit;
    private static int _playerCount;
    #endregion

    #region Game Statistics
    private static int _blueTeamScore, _redTeamScore;
    private static Dictionary<string, List<string>> _playerKills;
    private static Dictionary<string, int> _playerDeaths;
    #endregion
    #endregion

    #region Getters and Setters
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
            else if(value > 100)
            {
                _matchScoreLimit = 100;
            }
            else
            {
                _matchScoreLimit = value;
            }
        }
    }
    public static int matchTimeLimit
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
    #endregion
}
