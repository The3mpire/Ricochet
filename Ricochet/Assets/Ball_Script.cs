using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Script : MonoBehaviour {
    public Vector2 initialForce = new Vector2(255, 157);

    private Rigidbody2D body;
    private Vector2 current;
    // Use this for initialization
    void Start() {
        body = GetComponent<Rigidbody2D>();

        body.AddForce(initialForce);

        current = body.velocity;
    }

    // Update is called once per frame
    private void OnCollisionExit2D(Collision2D collision)
    {
        current = body.velocity;
    }
}