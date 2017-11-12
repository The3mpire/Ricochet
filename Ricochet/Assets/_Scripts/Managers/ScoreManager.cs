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
    #endregion

    #region Hidden Variables
    private int redTeamScore = 0;
    private int blueTeamScore = 0;
    private int scoreGoal = 2;
    #endregion

    public override bool UpdateScore(Enumerables.ETeam team, int value)
    {
        switch (team)
        {
            // it increments the opposing team (this way the inspector variables make more sense, 
            // and all the reverse logic is done in the code)
            case ETeam.RedTeam:
                blueTeamScore += value;
                BlueTeamText.text = blueTeamScore.ToString();
                return blueTeamScore % scoreGoal == 0;
            case ETeam.BlueTeam:
                redTeamScore += value;
                RedTeamText.text = redTeamScore.ToString();
                return redTeamScore % scoreGoal == 0;
        }
        return false;
    }
}
