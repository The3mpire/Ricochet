using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {

    private GameManager gameManagerInstance;

    private void OnCollisionEnter2D(Collision2D col)
    {
        Collider2D hitCollider = col.collider;
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            gameManagerInstance.KillZoneCollision(hitCollider.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            gameManagerInstance.KillZoneCollision(other.gameObject);
        }
    }
}
