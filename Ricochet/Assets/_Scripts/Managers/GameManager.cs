using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Inspector Variables
    [Header("Reference Variables")]
    [Tooltip("Drag the mode manager here")]
    [SerializeField]
    private ModeManager modeManager;
    [Tooltip("Drag the powerup manager here")]
    [SerializeField]
    private PowerUpManager powerUpManager;

    [Header("Timers")]
    [Tooltip("How long the power up takes to respawn in seconds")]
    [SerializeField]
    private float powerUpRespawnTime = 10f;
    [Tooltip("How long the players take to respawn in seconds")]
    [SerializeField]
    private float playerRespawnTime = 2f;
    [Tooltip("How long the ball takes to respawn in seconds")]
    [SerializeField]
    private float ballRespawnTime = 2f;
    [Tooltip("How long a player can hold a ball with CatchNThrow")]
    [SerializeField]
    private float ballHoldTime = 1f;

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

        Cursor.visible = false;
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

    #region UI Controls
    public void ExitLevel()
    {
        SceneManager.LoadSceneAsync(LevelIndex.MAIN_MENU);
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(LevelIndex.LEVEL_ONE);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void ChangeScene()
    {
        SceneManager.LoadSceneAsync(Random.Range(1, SceneManager.sceneCountInBuildSettings));
    }
    #endregion

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

        EPowerUp currentPowerUp = playerController.GetCurrentPowerUp();
        if (currentPowerUp != EPowerUp.None)
        {
            playerController.RemovePowerUp();
            switch (currentPowerUp)
            {
                case EPowerUp.Multiball:
                    SpawnMultipleBalls(ball);
                    break;
                case EPowerUp.CatchNThrow:
                    playerController.SetBallHeld(ball);
                    ball.SetHeld(true);
                    ball.transform.SetParent(playerController.GetShieldTransform());
                    StartCoroutine(DropBallCoroutine(playerController, ball));
                    break;
            }
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
        if (!modeManager.UpdateScore(team, value))
        {
            ball.GetComponent<Ball>().OnBallGoalCollision();
            ball.SetActive(false);
            RespawnBall(ball);
        }
        else
        {
            ChangeScene();
        }
    }

    public void PlayerPowerUpCollision(GameObject player, PowerUp powerUp)
    {
        PlayerController playerController;

        // If player is not cached, cache them
        if (!playerDictionary.TryGetValue(player, out playerController))
        {
            playerController = player.GetComponent<PlayerController>();
            playerDictionary.Add(player, playerController);
        }
        EPowerUp powerUpType = powerUp.GetPowerUpType();
        Color powerUpShieldColor = powerUpManager.GetPowerUpShieldColor(powerUpType);
        playerController.ReceivePowerUp(powerUpType, powerUpShieldColor);
    }

    public void KillZoneCollision(GameObject haplessSoul)
    {
        switch (haplessSoul.tag)
        {
            case "Player":
                PlayerController playerController;
                if (!playerDictionary.TryGetValue(haplessSoul, out playerController))
                {
                    playerController = haplessSoul.GetComponent<PlayerController>();
                    playerDictionary.Add(haplessSoul, playerController);
                }
                playerController.PlayerDead();
                StartCoroutine(RespawnPlayer(playerController));
                break;
            case "Ball":
                haplessSoul.GetComponent<Ball>().OnBallGoalCollision();
                haplessSoul.SetActive(false);
                RespawnBall(haplessSoul);
                break;
        }
        
    }

    #endregion

    #region Respawners

    public void RespawnBall(GameObject ball)
    {
        if (!ball.GetComponent<Ball>().GetTempStatus())
        {
            StartCoroutine(SpawnBallCoroutine(ball));
        }
    }

    public void RespawnPowerUp(GameObject powerUp)
    {
        StartCoroutine(RespawnPowerUpRoutine(powerUp));
    }

    private void SpawnMultipleBalls(Ball origBall)
    {
        Ball ball1 = Instantiate(origBall);
        Ball ball2 = Instantiate(origBall);

        ball1.SetTempStatus(true);
        ball2.SetTempStatus(true);
    }

    private IEnumerator SpawnBallCoroutine(GameObject ball)
    {
        yield return new WaitForSeconds(ballRespawnTime);

        ball.transform.position = ballRespawns[Random.Range(0, ballRespawns.Length)].position;

        ball.SetActive(true);
    }

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

    private IEnumerator RespawnPowerUpRoutine(GameObject powerUp)
    {
        yield return new WaitForSeconds(powerUpRespawnTime);
        powerUp.SetActive(true);
    }

    private IEnumerator DropBallCoroutine(PlayerController player, Ball ball)
    {
        yield return new WaitForSeconds(ballHoldTime);

        player.SetBallHeld(null);
        ball.SetHeld(false);
        ball.transform.SetParent(null, true);
    }
    #endregion

    #region Rerouters

    public Color GetPowerUpColor(EPowerUp powerup)
    {
        return powerUpManager.GetPowerUpColor(powerup);
    }

    public Color GetPowerUpShieldColor(EPowerUp powerup)
    {
        return powerUpManager.GetPowerUpShieldColor(powerup);
    }

    #endregion
}
