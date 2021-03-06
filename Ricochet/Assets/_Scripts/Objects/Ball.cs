﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("The ball travel direction on spawn")]
    [SerializeField]
    private Vector2 initialDirection = new Vector2(0, -1);

    [Tooltip("The slowest the ball can travel")]
    [SerializeField]
    private float minimumSpeed = 5f;
    [Tooltip("The fastest the ball can travel")]
    [SerializeField]
    private float maximumSpeed = 27f;

    [Tooltip("Enabled: the ball slows according to an inverse square curve\n" +
        "Disabled: the ball slows linearly")]
    [SerializeField]
    private bool curveSlow = false;
    [Tooltip("How quickly the ball slows to its normal speed after accelerating\n" +
        "Higher number means velocity decreases faster")]
    [SerializeField]
    private float slowRate = 1f;
    [Tooltip("Adjust if using curve slowing; ignored if not\n" +
        "Higher number means velocity decreases slower")]
    [SerializeField]
    private float curveModifier = 10f;


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
    [Tooltip("Drag the audio source here")]
    [SerializeField]
    private AudioSource audioSource;
    [Tooltip("Drag the trail particle system here")]
    [SerializeField]
    private ParticleSystem trail;
    #endregion

    #region Hidden Variables
    private GameManager gameManagerInstance;
    private LinkedList<PlayerController> lastTouchedBy;
    private ParticleSystem.MainModule psMain;
    private ParticleSystem.EmissionModule psEmission;
    private Vector3 lastPosition;
    private float curveTimer;
    private bool beenHit;
    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        lastTouchedBy = new LinkedList<PlayerController>();
        psMain = trail.main;
        psEmission = trail.emission;
        beenHit = false;

        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            if (!gameManagerInstance.GetBallObjects().Contains(gameObject))
            {
                gameManagerInstance.AddBallObject(gameObject);
            }
        }
    }

    private void Start()
    {
        if (isTempBall)
        {
            audioSource.mute = true;
            StartCoroutine(UnmuteSpawnedBall(.5f));
        }
    }

    private void OnEnable()
    {
        body.velocity = new Vector2(0.0f, 0.0f);
        curveTimer = slowRate;
        psMain = trail.main;
        psEmission = trail.emission;

        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            if (!gameManagerInstance.GetBallObjects().Contains(gameObject))
            {
                gameManagerInstance.AddBallObject(gameObject);
            }
        }
    }

    private void OnDisable()
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            gameManagerInstance.RemoveBallObject(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (beenHit)
        {
            lastPosition = transform.position;
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
                if (curveSlow)
                {
                    float velDiff = body.velocity.magnitude - minimumSpeed;
                    body.velocity = body.velocity.normalized * (body.velocity.magnitude - slowRate * velDiff * Time.deltaTime / curveModifier);
                }
                else
                {
                    body.velocity = body.velocity.normalized * (body.velocity.magnitude - slowRate * Time.deltaTime);
                }
            }
            psMain.startSpeed = body.velocity.magnitude / 3;
            psMain.startRotation = GetRotation();
            //psMain.startRotation = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            psEmission.rateOverTime = (body.velocity.magnitude * 10f);
        }
        else
        {
            psEmission.rateOverTime = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        curveTimer = slowRate;
        Collider2D hitCollider = col.collider;
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            //audioSource.PlayOneShot(gameManagerInstance.GetBallSound(hitCollider.tag, col.relativeVelocity.magnitude >= 35));
            audioSource.PlayOneShot(gameManagerInstance.GetBallSound(hitCollider.tag));
            beenHit = true;
            if (hitCollider.tag == "Shield")
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

    /*
     * Called once the ball kills a player
     * Could be used to adjust post-kill velocity or direction
     */
    public void RedirectBall(Vector2 relativeVelocity, Vector2 newDirection)
    {
        if (newDirection != Vector2.zero)
        {
            body.velocity = newDirection.normalized * relativeVelocity.magnitude;
        }
        else
        {
            body.velocity = body.velocity.normalized * relativeVelocity.magnitude;
        }
    }

    public void ResetPosition()
    {
        Vector2[] directions = { Vector2.up, new Vector2(1, 1), Vector2.right, new Vector2(1, -1),
            Vector2.down, new Vector2(-1, -1), Vector2.left, new Vector2(-1, 1) };
        Vector2 spawn = Vector2.zero;
        RaycastHit2D[] hits = new RaycastHit2D[8];
        float max = 0f;
        for (int r = 0; r < 8; r++)
        {
            hits[r] = Physics2D.Raycast(transform.position, directions[r], 30f, LayerMask.GetMask("Spawn"), -1, 1);
            if (hits[r].collider != null && hits[r].distance > max)
            {
                max = hits[r].distance;
                spawn = hits[r].point;
            }
        }
        if (spawn != Vector2.zero)
        {
            transform.position = spawn;
            body.velocity = -body.velocity;
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

    public bool GetBeenHit()
    {
        return beenHit;
    }

    public void SetBeenHit(bool value)
    {
        beenHit = value;
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

    #region Helpers
    private IEnumerator UnmuteSpawnedBall(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        audioSource.mute = false;
    }

    private float GetRotation()
    {
        float x = body.velocity.x;
        float y = body.velocity.y;
        float degree = 0;
        if (y == 0)
            return degree = x >= 0 ? 90f : 270f;

        degree = Vector2.Angle(new Vector2(1f, 0f), body.velocity) * Mathf.Deg2Rad;
        if (y > 0)
            degree *= -1;
        return degree;
    }

    #endregion
}
