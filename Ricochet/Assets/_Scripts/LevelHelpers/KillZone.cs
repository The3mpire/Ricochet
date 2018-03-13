using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{

    private GameManager gameManagerInstance;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            gameManagerInstance.KillZoneCollision(other.gameObject);
        }
    }
}
