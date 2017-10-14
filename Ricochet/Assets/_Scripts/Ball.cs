using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    //TODO make private this hurts
    public Vector2 initialForce = new Vector2(255, 157);
    public float resetDelay = 2f;

    public float minimumSpeed = 1f;
    public float speedUpForce = 2f;
    public float maximumSpeed = 10f;

    public bool isTempBall = false;
    public bool canScore = true;

    public SpriteRenderer sprite;
    public Rigidbody2D body;

    private Vector2 initialPosition;

    private bool hidden;

    private GameManager gameManagerInstance;

    #region MonoBehaviour

    private void Start()
    {
        initialPosition = transform.position;

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
        if (hitCollider.tag == "Player")
        {
            PlayerController pc = hitCollider.GetComponent<PlayerController>();

            //TODO go through game manager (which then goes to score manager)
            pc.gameObject.SetActive(false);
            //pc.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            //ScoreUI.Score(pc.teamNumber == 1 ? 'b' : 'r', -1);
            //StartCoroutine(RespawnPlayer(pc));
        }
        else if (hitCollider.tag == "Shield")
        {
            if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
            {
                gameManagerInstance.CheckBallShieldCollision(hitCollider.gameObject, this);
            }
        }

    }

    #endregion

    public void Reset()
    {
        if (!hidden)
        {
            SetVisible(false);
            if (!isTempBall)
            {
                StartCoroutine(DelayedStart());
            }
        }
    }

    private void SetVisible(bool value)
    {
        body.simulated = value;
        sprite.enabled = value;
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(resetDelay);

        transform.position = initialPosition;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        body.AddForce(initialForce);

        SetVisible(true);
    }
}