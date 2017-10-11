using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Tooltip("How long the power up takes to respawn in seconds")]
    public static float powerUpRespawn = 10f;

    public Transform[] teamOneRespawns;
    public Transform[] teamTwoRespawns;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        instance = this;
    }

    public void RespawnPowerUp(GameObject o)
    {
        StartCoroutine(RespawnPowerUpRoutine(o));
    }

    private IEnumerator RespawnPowerUpRoutine(GameObject pu)
    {
        yield return new WaitForSeconds(powerUpRespawn);
        pu.SetActive(true);
    }
}
