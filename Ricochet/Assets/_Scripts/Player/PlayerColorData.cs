using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class PlayerColorData : MonoBehaviour
{
    public static Color blueTeamPlayer1 = Color.cyan;
    public static Color blueTeamPlayer2 = Color.blue;
    public static Color redTeamPlayer1 = Color.red;
    public static Color redTeamPlayer2 = Color.magenta;

    public static Color getColor(int playerNum, ETeam team)
    {
        switch (team)
        {
            case ETeam.RedTeam:
                return playerNum % 2 == 0 ? redTeamPlayer1 : redTeamPlayer2;
            case ETeam.BlueTeam:
                return playerNum % 2 == 0 ? blueTeamPlayer1 : blueTeamPlayer2;
            default:
                return Color.white;
        }
    }
        
}
    

