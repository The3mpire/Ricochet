using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enumerables;

public class GameManager : MonoBehaviour
{
    #region Inspector Variables
    [Header("Reference Variables")]
    [Tooltip("Drag the mode manager here")]
    [SerializeField]
    private ModeManager modeManager;

    [Header("Game Match Variables")]
    [Tooltip("How long the selected game lasts in seconds")]
    [SerializeField]
    private float gameMatchTime = 120f;
    [Tooltip("Drag the timer from the UI screen here")]
    [SerializeField]
    private Text gameTimerText;
    [Tooltip("Drag the Game Menu UI here")]
    [SerializeField]
    private Canvas gameMenuUI;
    [Tooltip("Drag the winning team text here")]
    [SerializeField]
    private Text winningTeamText;

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

    private float currentMatchTime = 0.0f;
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

    void Update()
    {
        matchTimer();
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
    }

    public void BallSecondaryShieldCollision(GameObject secondaryShield, Ball ball)
    {
        PlayerController playerController = secondaryShield.GetComponent<Shield>().GetPlayer();
        switch (playerController.GetCurrentPowerUp())
        {
            case EPowerUp.CircleShield:
                playerController.EnableSecondaryShield(false);
                playerController.RemovePowerUp();
                break;
        }
    }

    public void BallPlayerCollision(GameObject player, Ball ball)
    {
        PlayerController playerController;

        if (!playerDictionary.TryGetValue(player, out playerController))
        {
            playerController = player.GetComponent<PlayerController>();
            playerDictionary.Add(player, playerController);
        }

        playerController.KillPlayer();
        StartCoroutine(RespawnPlayer(playerController));
    }

    public void BallGoalCollision(GameObject ball, ETeam team, int value)
    {
        modeManager.UpdateScore(team, value);
        ball.SetActive(false);
        RespawnBall(ball);
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

    private void matchTimer()
    {
        if (currentMatchTime < gameMatchTime)
        {
            currentMatchTime += Time.deltaTime;
            string minutes = ((int)(currentMatchTime / 60)).ToString();
            int seconds_num = (int)(currentMatchTime % 60);
            string seconds;
            if (seconds_num < 10)
            {
                seconds = '0' + seconds_num.ToString();
            }
            else
            {
                seconds = seconds_num.ToString();
            }
            gameTimerText.text = minutes + ':' + seconds;
        }
        else
        {
            //if we want to disable the players during the win screen not sure exactly what to do here so need some team input
            foreach (KeyValuePair<GameObject, PlayerController> player in playerDictionary)
            {
                //player.Value.enabled = false;
            }
            Enumerables.ETeam winningTeam = modeManager.ReturnWinningTeam();
            Debug.Log(winningTeam);
            gameMenuUI.gameObject.SetActive(true);
            winningTeamText.gameObject.SetActive(true);
            if (winningTeam == ETeam.BlueTeam)
            {
                winningTeamText.text = "Congradulations Blue Team!";
                winningTeamText.color = Color.blue;
            }
            else if (winningTeam == ETeam.RedTeam)
            {
                winningTeamText.text = "Congradulations Red Team!";
                winningTeamText.color = Color.red;
            }
            else
            {
                winningTeamText.text = "Draw...";
                winningTeamText.color = Color.white;
            }
            //start the coorutine that will wait a few seconds so this is displayed and then switch scene
        }
    }

    #endregion
}
