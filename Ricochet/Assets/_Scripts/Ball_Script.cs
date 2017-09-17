using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Script : MonoBehaviour {
    public Vector2 initialForce = new Vector2(255, 157);
    public float resetDelay = 2f;

    private Rigidbody2D body;
    private Vector2 start;
    private SpriteRenderer sprite;
    // should I be using a lock?
    private bool hidden;
    // Use this for initialization
    void Start() {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        start = transform.position;

        body.AddForce(initialForce);
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