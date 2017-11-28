using System.Collections;
using System.Collections.Generic;

public static class GameData {
    #region Private
    #region Game Setup
    private static int _matchScoreLimit;
    private static int _matchTimeLimit;
    private static int _playerCount;
    private static Enumerables.ECharacter _p1Character;
    private static Enumerables.ECharacter _p2Character;
    private static Enumerables.ECharacter _p3Character;
    private static Enumerables.ECharacter _p4Character;
    private static Enumerables.ETeam _p1Team;
    private static Enumerables.ETeam _p2Team;
    private static Enumerables.ETeam _p3Team;
    private static Enumerables.ETeam _p4Team;
    #endregion

    #region Game Statistics
    private static int _blueTeamScore, _redTeamScore;
    private static Enumerables.ETeam _gameWinner;
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
    public static Enumerables.ECharacter p1Character
    {
        get { return _p1Character; }
        set { _p1Character = value; }
    }
    public static Enumerables.ECharacter p2Character
    {
        get { return _p2Character; }
        set { _p2Character = value; }
    }
    public static Enumerables.ECharacter p3Character
    {
        get { return _p3Character; }
        set { _p3Character = value; }
    }
    public static Enumerables.ECharacter p4Character
    {
        get { return _p4Character; }
        set { _p4Character = value; }
    }
    public static Enumerables.ETeam p1Team
    {
        get { return _p1Team; }
        set { _p1Team = value; }
    }
    public static Enumerables.ETeam p2Team
    {
        get { return _p2Team; }
        set { _p2Team = value; }
    }
    public static Enumerables.ETeam p3Team
    {
        get { return _p3Team; }
        set { _p3Team = value; }
    }
    public static Enumerables.ETeam p4Team
    {
        get { return _p4Team; }
        set { _p4Team = value; }
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
    #endregion

    #region Functions
    public static void ResetGameSetup()
    {
        _matchScoreLimit = 0;
        _matchTimeLimit = 0;
        _playerCount = 0;
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
