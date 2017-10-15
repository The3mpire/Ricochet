using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("How hard the ball was hit when it spawns")]
    [SerializeField]
    private Vector2 initialForce = new Vector2(255, 157);

    [Tooltip("The slowest the ball can travel")]
    [SerializeField]
    private float minimumSpeed = 1f;
    [Tooltip("How much the ball gains speed when just bouncing around")]
    [SerializeField]
    private float speedUpForce = 2f;
    [Tooltip("The fastest the ball can travel")]
    [SerializeField]
    private float maximumSpeed = 10f;

    [Tooltip("Whether this ball should respawn")]
    [SerializeField]
    private bool isTempBall = false;
    [Tooltip("Whether this ball can score goals")]
    [SerializeField]
    private bool canScore = true;

    [Tooltip("Drag the ball here")]
    [SerializeField]
    private Rigidbody2D body;
    #endregion

    private GameManager gameManagerInstance;

    #region MonoBehaviour

    private void OnEnable()
    {
        body.velocity = Vector3.zero;
        body.AddForce(initialForce);
    }

    private void FixedUpdate()
    {
        // add the constant force
        if (body.velocity.magnitude < minimumSpeed)
        {
            body.AddForce(body.velocity.normalized * speedUpForce);
        }

        // make sure we're not going super mega fast
        if (body.velocity.magnitude > maximumSpeed)
        {
            body.velocity = body.velocity.normalized * maximumSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Collider2D hitCollider = col.collider;
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            if (hitCollider.tag == "Player")
            {
                gameManagerInstance.BallPlayerCollision(hitCollider.gameObject, this);

            }
            else if (hitCollider.tag == "Shield")
            {
                gameManagerInstance.BallShieldCollision(hitCollider.gameObject, this);
            }
        }

    }

    #endregion

    #region Getters and Setters
    public bool GetTempStatus()
    {
        return isTempBall;
    }

    public void SetTempStatus(bool status)
    {
        isTempBall = status;
    }

    public bool GetCanScore()
    {
        return canScore;
    }

    public void SetCanScore(bool value)
    {
        canScore = value;
    }
    #endregion
}