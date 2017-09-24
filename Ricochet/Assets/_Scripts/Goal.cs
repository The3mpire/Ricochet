using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {
    public Text Score;
    private int _score = 0;

    private void Start()
    {
        if (Score == null)
        {
            throw new System.Exception("No score text object assigned to goal");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            Score.text = (++_score).ToString();
            collision.GetComponent<BallMovement>().Reset();
        }
    }
}
