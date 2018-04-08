using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuBall : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("The ball travel direction on spawn")]
    [SerializeField]
    private Vector2 initialDirection = new Vector2(0, -1);
    [Tooltip("The slowest the ball can travel")]
    [SerializeField]
    private float minimumSpeed = 20f;
    [Tooltip("The fastest the ball can travel")]
    [SerializeField]
    private float maximumSpeed = 30f;

    [Tooltip("Drag the ball here")]
    [SerializeField]
    private Rigidbody2D body;
    [Tooltip("Drag the audio source here")]
    [SerializeField]
    private AudioSource audioSource;
    [Tooltip("Drag the sound storage here")]
    [SerializeField]
    private SoundStorage soundStorage;
    #endregion

    #region MonoBehaviour
   void Start()
    {
        body.AddForce(initialDirection);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Collider2D hitCollider = col.collider;
        audioSource.PlayOneShot(soundStorage.GetBallSound(hitCollider.tag, col.relativeVelocity.magnitude >= 35));
    }

    void LateUpdate()
    {
        if (body.velocity.magnitude < minimumSpeed)
        {
            body.velocity = body.velocity.normalized * minimumSpeed;
        }
        // make sure we're not going super mega fast
        else if (body.velocity.magnitude > maximumSpeed)
        {
            body.velocity = body.velocity.normalized * maximumSpeed;
        }
    }

    #endregion
}
