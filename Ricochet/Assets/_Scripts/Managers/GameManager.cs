using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    [Tooltip("How long the power up takes to respawn in seconds")]
    [SerializeField]
    private static float powerUpRespawn = 10f;
    [Header("Team Respawns")]
    [SerializeField]
    private Transform[] teamOneRespawns;
    [SerializeField]
    private Transform[] teamTwoRespawns;

    // dictionary of players cached based off the GameObject
    private Dictionary<GameObject, PlayerController> playerDictionary = new Dictionary<GameObject, PlayerController>();

    #region MonoBehaviour
    void Awake()
    {
        if (instance != null && instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        instance = this;
    }
    #endregion

    #region Helpers
    public static bool TryGetInstance(out GameManager gm)
    {
        gm = instance;
        if(instance == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
#endregion

    #region PowerUps

    public void CheckBallShieldCollision(GameObject shield, Ball ball)
    {
        PlayerController playerController;

        // check if the player is cached / cache it
        if(!playerDictionary.TryGetValue(shield, out playerController))
        {
            playerController = shield.GetComponent<Shield>().GetPlayer();
            playerDictionary.Add(shield, playerController);
        }


        switch (playerController.GetCurrentPowerUp())
        {
            case EPowerUp.Multiball:
                SpawnBalls(ball);
                break;
        }

        //    PlayerController player = hitCollider.gameObject.GetComponentInParent<PlayerController>();
        //    //see if they have the power up
        //    if (player.hasPowerUp)
        //    {
        //        SpawnBalls();
        //        player.hasPowerUp = false;
        //        player.shield.color = Color.white;
        //    }

    }

    public void CheckBallPlayerCollision(GameObject player, Ball ball)
    {

    }


    public void RespawnPowerUp(GameObject o)
    {
        StartCoroutine(RespawnPowerUpRoutine(o));
    }

    private void SpawnBalls(Ball origBall)
    {
        Ball ball1 = Instantiate(origBall);
        Ball ball2 = Instantiate(origBall);

        ball1.isTempBall = true;
        ball2.isTempBall = true;
    }

    private IEnumerator RespawnPowerUpRoutine(GameObject powerUp)
    {
        yield return new WaitForSeconds(powerUpRespawn);
        powerUp.SetActive(true);
    }
    #endregion
}
