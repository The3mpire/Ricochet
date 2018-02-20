using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishObject : MonoBehaviour
{
    #region Private
    private GameManager gameManager;
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        gameManager = GetComponentInParent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameManager != null || GameManager.TryGetInstance(out gameManager))
        {
            gameManager.KillZoneCollision(collision.gameObject);
        }
    }
    #endregion
}
