using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour {
    public Vector2 initialForce = new Vector2(255, 157);
    public float resetDelay = 2f;

    public float minimumSpeed = 1f;
    public float speedUpForce = 2f;
    public float maximumSpeed = 10f;

    public bool canScore = true;

    private Rigidbody2D body;
    private Vector2 start;
    private SpriteRenderer sprite;
    
    private bool hidden;
    // Use this for initialization
    void Start() {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        start = transform.position;

        body.AddForce(initialForce);
    }

    void FixedUpdate()
    {
        // add the constant force
        if(body.velocity.magnitude < minimumSpeed)
        {
            body.AddForce(body.velocity.normalized * speedUpForce);
        }

        // make sure we're not going super mega fast
        if (body.velocity.magnitude > maximumSpeed)
        {
            body.velocity = body.velocity.normalized * maximumSpeed;
        }
    }

    public void Reset()
    {
        if (!hidden)
        {
            SetVisible(false);
            StartCoroutine(DelayedStart());
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

        transform.position = start;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        body.AddForce(initialForce);

        SetVisible(true);
    }
}