﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ball : MonoBehaviour {

	#region Inspector Variables
	[Tooltip("The ball travel direction on spawn")]
	[SerializeField]
	private Vector2 initialDirection = new Vector2(0, -1);

	[Tooltip("The slowest the ball can travel")]
	[SerializeField]
	private float minimumSpeed = 5f;
	[Tooltip("How quickly the ball slows to its normal speed after accelerating")]
	[SerializeField]
	private float slowRate = 1f;
	[Tooltip("The fastest the ball can travel")]
	[SerializeField]
	private float maximumSpeed = 27f;

	[Tooltip("Whether this ball should respawn")]
	[SerializeField]
	private bool isTempBall = false;
	[Tooltip("Whether this ball can score goals")]
	[SerializeField]
	private bool canScore = true;

	[Tooltip("How many players back to last touch the ball.")]
	[SerializeField]
	private int lastTouchedByCount = 2;

	[Tooltip("Drag the ball here")]
	[SerializeField]
	private Rigidbody2D body;
	#endregion

	#region Hidden Variables
	private GameManager gameManagerInstance;
	private LinkedList<PlayerController> lastTouchedBy;
	#endregion

	#region MonoBehaviour

	private void Awake()
	{
		lastTouchedBy = new LinkedList<PlayerController>();
	}

	private void OnEnable()
	{
		body.velocity = initialDirection.normalized * minimumSpeed;
	}

	private void FixedUpdate()
	{
		// add the constant force
		if (body.velocity.magnitude < minimumSpeed)
		{
			body.velocity = body.velocity.normalized * minimumSpeed;
		}
		// make sure we're not going super mega fast
		else if (body.velocity.magnitude > maximumSpeed)
		{
			body.velocity = body.velocity.normalized * maximumSpeed;
		}
		else if (body.velocity.magnitude > minimumSpeed)
		{
			body.velocity = body.velocity.normalized * (body.velocity.magnitude - slowRate * Time.deltaTime);
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
            else if (hitCollider.tag == "SecondaryShield")
            {
                gameManagerInstance.BallSecondaryShieldCollision(hitCollider.gameObject, this);
            }
        }
    }

	#endregion

	#region External Functions
	public void OnBallGoalCollision()
	{
		gameObject.SetActive(false);
		lastTouchedBy.Clear();
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

    public void SetHeld(bool held)
    {
        if (held)
        {
            body.bodyType = RigidbodyType2D.Kinematic;
            body.simulated = false;
        }
        else
        {
            body.bodyType = RigidbodyType2D.Dynamic;
            body.simulated = true;
        }
    }

    public void SetLastTouchedBy(PlayerController player)
	{
		if (lastTouchedBy.Count == 0 || player != lastTouchedBy.Last.Value)
		{
			lastTouchedBy.AddLast(player);
		}
		if (lastTouchedBy.Count > lastTouchedByCount)
		{ 
			lastTouchedBy.RemoveFirst();
		}
	}

	// If player isn't null, don't return the player passed in unless it is the only one in the list.
	// returns null if no one has touched the ball yet
	public PlayerController GetLastTouchedBy(PlayerController player = null)
	{
		if (lastTouchedBy.Count == 0)
		{
			return null;
		}
		if (player == null)
		{
			return lastTouchedBy.Last.Value;
		}
		else
		{
			foreach (PlayerController p in lastTouchedBy.Reverse())
			{
				if (p != player)
				{
					return p;
				}
			}
		}
		return player;
	}

	#endregion
}
