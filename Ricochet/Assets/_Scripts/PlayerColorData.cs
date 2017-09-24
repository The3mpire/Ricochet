using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorData : MonoBehaviour
{ 
    public static Color team1player1 = Color.cyan;
    public static Color team1player2 = Color.blue;
    public static Color team2player1 = Color.red;
    public static Color team2player2 = Color.magenta;

    public static Color getColor(int playerNum, int teamNum)
    {
        Debug.Log(playerNum);
        if(teamNum == 1)
        {
            if(playerNum % 2 == 0)
            {
                Debug.Log("in asdkljfal;sjfal;sjf");
                return team1player1;
            }
            else
            {
                return team1player2;
            }
        }
        else
        {
            if (playerNum % 2 == 0)
            {
                return team2player1;
            }
            else
            {
                return team2player2;
            }
        }
    }
    
}
