using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class PlayerColorData : MonoBehaviour
{
    private static Color blueTeamPlayer1 = Color.cyan;
    private static Color blueTeamPlayer2 = Color.blue;
    private static Color redTeamPlayer1 = Color.red;
    private static Color redTeamPlayer2 = Color.magenta;

    public static Color getColor(int playerNum, ETeam team)
    {
       /* switch (team)
        {
            case ETeam.RedTeam:
                return playerNum % 2 == 0 ? redTeamPlayer1 : redTeamPlayer2;
            case ETeam.BlueTeam:
                return playerNum % 2 == 0 ? blueTeamPlayer1 : blueTeamPlayer2;
            default:
                return Color.white;
        }*/
		return Color.white;
    }
        
}
    

