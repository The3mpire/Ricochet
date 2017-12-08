using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Drag Score Manager here")]
    [SerializeField]
    private static ScoreManager scoreManager = null;
    [Tooltip("The team the goal is for")]
    [SerializeField]
    private Enumerables.ETeam team;
    [Tooltip("How many points the team gets for scoring")]
    [SerializeField]
    private int points = 1;
    [Tooltip("Drag the audio source here")]
    [SerializeField]
    private AudioSource audioSource;
    #endregion

    #region Hidden Variables
    private GameManager gameManagerInstance = null;
    #endregion
    #region MonoBehaviour
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            // can't get the component early because not sure if it was a ball
            if (collider.tag == "Ball" && collider.GetComponent<Ball>().GetCanScore())
            {
                Ball ball = collider.GetComponent<Ball>();
                audioSource.PlayOneShot(gameManagerInstance.GetScoringSound());
                gameManagerInstance.BallGoalCollision(collider.gameObject, team, points);
            }
        }
    }
    #endregion
}
