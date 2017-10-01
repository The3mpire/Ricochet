using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public Text RedScore;
    public Text BlueScore;

    private int _blueScore = 0;
    private int _redScore = 0;
	// Use this for initialization
	void Start () {
		if (RedScore == null || BlueScore == null)
        {
            Debug.LogError("Score UI does not have a link to a score UI text");
        }
	}
	
    /// <summary>
    /// Incriments score for a specific team by a given value
    /// </summary>
    /// <param name="team">'b' for blue 'r' for red</param>
    /// <param name="value">points to be awarded</param>
	public void Score(char team, int value)
    {
        if (team == 'b')
        {
            _blueScore = value + _blueScore > 0 ? value + _blueScore : 0;
            BlueScore.text = _blueScore.ToString();
        }
        else if (team == 'r')
        {
            _redScore = value + _redScore > 0 ? value + _redScore : 0;
            RedScore.text = _redScore.ToString();
        }
        else
        {
            Debug.LogWarning("Score expected 'b' or 'r' as a team, was given: " + team);
        }
    }
}
