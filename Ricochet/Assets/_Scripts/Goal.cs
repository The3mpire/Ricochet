using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {
    public ScoreManager ScoreUI;
    /// <summary>
    /// Must be 'b' or 'r'
    /// Blue team's goal should have a 'r' and red team a 'b'
    /// This prevents the need to invert it when passing to ScoreUI.Score(_, _);
    /// </summary>
    public char OpposingTeam = '\0';

    private void Start()
    {
        if (ScoreUI == null)
        {
            Debug.LogError("No score manager script linked to goal");
        }

        OpposingTeam = char.ToLower(OpposingTeam);
        if (OpposingTeam == '\0')
        {
            Debug.LogError("Goal not assigned team");
        }
        else if(OpposingTeam != 'b' && OpposingTeam != 'r')
        {
            Debug.LogWarning("Goal given improper team character (must be 'b' or 'r'):" + OpposingTeam);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BallMovement ball = collision.GetComponent<BallMovement>();
        if (collision.tag == "Ball" && ball.canScore)
        {
            ScoreUI.Score(OpposingTeam, 1);
            ball.Reset();
        }
    }
}
