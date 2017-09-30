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
            Debug.LogError("No score text object assigned to goal", gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BallMovement ball = collision.GetComponent<BallMovement>();
        if (collision.tag == "Ball" && ball.canScore)
        {
            Score.text = (++_score).ToString();
            ball.Reset();
        }
    }
}
