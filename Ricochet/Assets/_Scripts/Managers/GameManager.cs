using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class GameManager : MonoBehaviour
{
    #region Inspector Variables
    [Header("Reference Variables")]
    [Tooltip("Drag the mode manager here")]
    [SerializeField]
    private ModeManager modeManager;

    [Header("Respawn Timers")]
    [Tooltip("How long the power up takes to respawn in seconds")]
    [SerializeField]
    private float powerUpRespawnTime = 10f;
    [Tooltip("How long the players take to respawn in seconds")]
    [SerializeField]
    private float playerRespawnTime = 2f;
    [Tooltip("How long the ball takes to respawn in seconds")]
    [SerializeField]
    private float ballRespawnTime = 2f;
    [Header("Respawn Zones")]
    [Tooltip("Locations for where Red Team can spawn")]
    [SerializeField]
    private Transform[] redTeamRespawns;
    [Tooltip("Locations for where Blue Team can spawn")]
    [SerializeField]
    private Transform[] blueTeamRespawns;
    [Tooltip("Locations where the ball can spawn")]
    [SerializeField]
    private Transform[] ballRespawns;
    #endregion

    #region Hidden Variables
    private static GameManager instance = null;

    // dictionary of players cached based off the GameObject
    private Dictionary<GameObject, PlayerController> playerDictionary = new Dictionary<GameObject, PlayerController>();
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;
    }
    #endregion

    public static bool TryGetInstance(out GameManager gm)
    {
        gm = instance;
        if (instance == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #region Collision Management

    public void BallShieldCollision(GameObject shield, Ball ball)
    {
        PlayerController playerController;
        
        // check if the player is cached / cache it
        if (!playerDictionary.TryGetValue(shield, out playerController))
        {
            playerController = shield.GetComponent<Shield>().GetPlayer();
            playerDictionary.Add(shield, playerController);
        }

        switch (playerController.GetCurrentPowerUp())
        {
            case EPowerUp.Multiball:
                playerController.RemovePowerUp();
                SpawnMultipleBalls(ball);
                break;
        }

        // The last player to touch the ball 
        ball.SetLastTouchedBy(playerController);
    }

    public void BallPlayerCollision(GameObject player, Ball ball)
    {
        PlayerController playerController;
        if (!playerDictionary.TryGetValue(player, out playerController))
        {
            playerController = player.GetComponent<PlayerController>();
            playerDictionary.Add(player, playerController);
        }

        // Check if the ball has been touched by anyone
        PlayerController lastTouchedBy = ball.GetLastTouchedBy(playerController);
        if (lastTouchedBy != null)
        {
            lastTouchedBy.RegisterKill(playerController);
        }

        playerController.PlayerDead();
        StartCoroutine(RespawnPlayer(playerController));
    }

    public void BallGoalCollision(GameObject ball, ETeam team, int value)
    {
        modeManager.UpdateScore(team, value);
        ball.GetComponent<Ball>().OnBallGoalCollision();
        RespawnBall(ball);
    }

    #endregion

    #region Respawners

    public void RespawnBall(GameObject ball)
    {
        if(!ball.GetComponent<Ball>().GetTempStatus())
        {
            StartCoroutine(SpawnBallCoroutine(ball));
        }
    }

    public void RespawnPowerUp(GameObject powerUp)
    {
        StartCoroutine(RespawnPowerUpRoutine(powerUp));
    }


    #endregion

    #region Private Helpers

    private IEnumerator RespawnPlayer(PlayerController playerController)
    {
        yield return new WaitForSeconds(playerRespawnTime);
        playerController.gameObject.SetActive(true);

        switch (playerController.GetTeamNumber())
        {
            case ETeam.RedTeam:
                playerController.transform.position = redTeamRespawns[Random.Range(0, redTeamRespawns.Length)].position;
                playerController.transform.rotation = redTeamRespawns[Random.Range(0, redTeamRespawns.Length)].rotation;
                break;
            case ETeam.BlueTeam:
                playerController.transform.position = blueTeamRespawns[Random.Range(0, redTeamRespawns.Length)].position;
                playerController.transform.rotation = blueTeamRespawns[Random.Range(0, redTeamRespawns.Length)].rotation;
                break;
        }
    }

    private void SpawnMultipleBalls(Ball origBall)
    {
        Ball ball1 = Instantiate(origBall);
        Ball ball2 = Instantiate(origBall);

        ball1.SetTempStatus(true);
        ball2.SetTempStatus(true);
    }

    private IEnumerator RespawnPowerUpRoutine(GameObject powerUp)
    {
        yield return new WaitForSeconds(powerUpRespawnTime);
        powerUp.SetActive(true);
    }

    private IEnumerator SpawnBallCoroutine(GameObject ball)
    {
        yield return new WaitForSeconds(ballRespawnTime);

        ball.transform.position = ballRespawns[Random.Range(0, ballRespawns.Length)].position;

        ball.SetActive(true);
    }

    #endregion
}
