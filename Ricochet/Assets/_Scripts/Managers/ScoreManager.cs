using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Enumerables;

public class ScoreManager : ModeManager
{
    #region Inspector Variables
    [Tooltip("Drag the Score UI's Red Team's Text here")]
    [SerializeField]
    private Text RedTeamText;
    [Tooltip("Drag the Score UI's Team One Text here")]
    [SerializeField]
    private Text BlueTeamText;
    [Tooltip("Max score per scene")]
    [SerializeField]
    private int scoreGoal = 20;
    #endregion

    #region Hidden Variables
    private int redTeamScore = 0;
    private int blueTeamScore = 0;
    #endregion

    public override bool UpdateScore(Enumerables.ETeam team, int value)
    {
        switch (team)
        {
            // it increments the opposing team (this way the inspector variables make more sense, 
            // and all the reverse logic is done in the code)
            case ETeam.RedTeam:
                blueTeamScore += value;
                GameData.blueTeamScore += value;
                BlueTeamText.text = blueTeamScore.ToString();
                return CheckWin(GameData.blueTeamScore);
            case ETeam.BlueTeam:
                redTeamScore += value;
                GameData.redTeamScore += value;
                RedTeamText.text = redTeamScore.ToString();
                return CheckWin(GameData.redTeamScore);
        }
        return false;
    }

    public bool CheckWin(int score)
    {
        if (GameData.matchScoreLimit > 0)
        {
            return score % GameData.matchScoreLimit == 0;
        }
        else
        {
            return false;
        }
    }
}
