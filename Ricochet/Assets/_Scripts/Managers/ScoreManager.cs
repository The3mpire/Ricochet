using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Enumerables;

public class ScoreManager : ModeManager
{
    #region Inspector Variables
    [SerializeField]
    private GameDataSO gameData;

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
    #endregion

    public override bool UpdateScore(Enumerables.ETeam team, int value)
    {
        switch (team)
        {
            // it increments the opposing team (this way the inspector variables make more sense, 
            // and all the reverse logic is done in the code)
            case ETeam.RedTeam:
                blueTeamScore += value;
                gameData.SetBlueScore(blueTeamScore);
                BlueTeamText.text = blueTeamScore.ToString();
                return CheckWin(blueTeamScore);
            case ETeam.BlueTeam:
                redTeamScore += value;
                gameData.SetRedScore(redTeamScore);
                RedTeamText.text = redTeamScore.ToString();
                return CheckWin(redTeamScore);
        }
        return false;
    }

    public override bool AltUpdateScore(ETeam team, int value)
    {
        switch (team)
        {
            // it increments team's score
            case ETeam.RedTeam:
                redTeamScore += value;
                gameData.SetRedScore(redTeamScore);
                RedTeamText.text = redTeamScore.ToString();
                return CheckWin(redTeamScore);
            case ETeam.BlueTeam:
                blueTeamScore += value;
                gameData.SetBlueScore(blueTeamScore);
                BlueTeamText.text = blueTeamScore.ToString();
                return CheckWin(blueTeamScore);
        }
        return false;
    }

    public override ETeam GetMaxScore()
    {
        if (redTeamScore > blueTeamScore)
        {
            return ETeam.RedTeam;
        }
        else if (blueTeamScore > redTeamScore)
        {
            return ETeam.BlueTeam;
        }
        else
        {
            return ETeam.None;
        }
    }

    public bool CheckWin(int score)
    {
        if (gameData.GetScoreLimit() > 0 && score > 0)
        {
            return score % gameData.GetScoreLimit() == 0;
        }
        else
        {
            return false;
        }
    }
}
