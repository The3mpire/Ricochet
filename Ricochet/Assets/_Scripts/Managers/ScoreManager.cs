using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Enumerables;

public class ScoreManager : ModeManager
{
    [Tooltip("Drag the Score UI's Red Team's Text here")]
    [SerializeField]
    private Text RedTeamText;
    [Tooltip("Drag the Score UI's Team One Text here")]
    [SerializeField]
    private Text BlueTeamText;

    private int redTeamScore = 0;
    private int blueTeamScore = 0;

    public override void UpdateScore(Enumerables.ETeam team, int value)
    {
        switch (team)
        {
            // it increments the opposing team (this way the inspector variables make more sense, 
            // and all the reverse logic is done in the code)
            case ETeam.RedTeam:
                blueTeamScore += value;
                BlueTeamText.text = blueTeamScore.ToString();
                break;
            case ETeam.BlueTeam:
                redTeamScore += value;
                RedTeamText.text = redTeamScore.ToString();
                break;
        }
    }
}
