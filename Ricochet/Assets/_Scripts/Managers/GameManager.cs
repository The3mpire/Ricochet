using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enumerables;
using UnityEngine.SceneManagement;
using PlayerSelectData;

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

    [Header("Game Match Variables")]
    [Tooltip("Drag the Game Menu UI here(Can be null if there is no timer)")]
    [SerializeField]
    private Canvas gameMenuUI;
    [Tooltip("How long the selected game lasts in seconds")]
    [SerializeField]
    private float gameMatchTime = 120f;
    [Tooltip("Drag the timer from the UI screen here")]
    [SerializeField]
    private Text gameTimerText;
    [Tooltip("Drag the winning team text here")]
    [SerializeField]
    private Text winningTeamText;
    [Tooltip("Time to wait for before switching back to game select menu (in seconds)")]
    [SerializeField]
    private int winScreenWaitTime = 3;

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

    [Header("Players in match")]
    [Tooltip("The gameobject players in the match")]
    [SerializeField]
    private GameObject[] players;

    [Header("Character Variables")]
    [Tooltip("Image of first playable character")]
    [SerializeField]
    private Sprite character1Sprite;
    [Tooltip("Scale of the character 1 so the body image fits correctly")]
    [SerializeField]
    private float character1BodyScale;
    [Tooltip("Image of second playable character")]
    [SerializeField]
    private Sprite character2Sprite;
    [Tooltip("Scale of the character 2 so the body image fits correctly")]
    [SerializeField]
    private float character2BodyScale;

    [Tooltip("Score limit to win the match")]
    [SerializeField]
    private int scoreLimit;
    [Tooltip("Time limit for the match")]
    [SerializeField]
    private int timeLimit;
    #endregion

    #region Hidden Variables

    private static GameManager instance = null;

    private static Dictionary<int, PlayerData> playerSelectedData = null;

    // dictionary of players cached based off the GameObject
    private Dictionary<GameObject, PlayerController> playerDictionary = new Dictionary<GameObject, PlayerController>();

    private float currentMatchTime;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        currentMatchTime = gameMatchTime;
        if (playerSelectedData != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                PlayerController currentPlayer = players[i].GetComponent<PlayerController>();
                currentPlayer.SetPlayerNumber(i + 1);
                if (playerSelectedData[i + 1].ready)
                {
                    if (playerSelectedData[i + 1].team == 1)
                    {
                        currentPlayer.SetTeam(ETeam.RedTeam);
                    }
                    else
                    {
                        currentPlayer.SetTeam(ETeam.BlueTeam);
                    }
                    if (playerSelectedData[i + 1].character == "bean")
                    {
                        currentPlayer.SetBodyType(character1Sprite);
                        currentPlayer.SetBodyScale(character1BodyScale);
                    }
                    else
                    {
                        currentPlayer.SetBodyType(character2Sprite);
                        currentPlayer.SetBodyScale(character2BodyScale);
                    }
                    NoWaitRespawnPlayer(currentPlayer);
                }
                else
                {
                    players[i].SetActive(false);
                }
            }
            playerSelectedData = null;
        }

        instance = this;

        Cursor.visible = false;
        GameData.matchScoreLimit = scoreLimit;
        GameData.matchTimeLimit = timeLimit;
    }

    void Update()
    {
        if (gameMenuUI != null)
        {
            MatchTimer();
        }
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

    public void CharacterSelect()
    {
        SceneManager.LoadSceneAsync(LevelIndex.CHARACTER_SELECT);
    }

    public void StartGame(Dictionary<int, PlayerData> playerData)
    {
        playerSelectedData = playerData;
        AsyncOperation async = SceneManager.LoadSceneAsync(LevelIndex.UP_N_OVER);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void ChangeScene()
    {
        SceneManager.LoadSceneAsync(Random.Range(1, SceneManager.sceneCountInBuildSettings));
    }

    private void EndMatch()
    {
        SceneManager.LoadSceneAsync("EndGame");
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
            switch (currentPowerUp)
            {
                case EPowerUp.Multiball:
                    playerController.RemovePowerUp();
                    SpawnMultipleBalls(ball);
                    break;
                case EPowerUp.CatchNThrow:
                    playerController.RemovePowerUp();
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

        // Check if the ball has been touched by anyone
        PlayerController lastTouchedBy = ball.GetLastTouchedBy(playerController);
        if (lastTouchedBy != null)
        {
            lastTouchedBy.RegisterKill(playerController);
        }

        playerController.PlayerDead();
        StartCoroutine(RespawnPlayer(playerController));
    }

    public void BallGoalCollision(GameObject ball, ETeam team, int points)
    {
        if (!modeManager.UpdateScore(team, points))
        {
            ball.GetComponent<Ball>().OnBallGoalCollision();
            ball.SetActive(false);
            RespawnBall(ball);
        }
        else
        {
            GameData.gameWinner = team;
            EndMatch();
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
        if (playerController.GetCurrentPowerUp() != EPowerUp.None)
        {
            playerController.RemovePowerUp();
        }
        EPowerUp powerUpType = powerUp.GetPowerUpType();
        Color powerUpShieldColor = powerUpManager.GetPowerUpShieldColor(powerUpType);
        playerController.ReceivePowerUp(powerUpType, powerUpShieldColor);
        if (powerUpType == EPowerUp.CircleShield) //need to have a list of powerups that reference a secondary shield
        {
            playerController.EnableSecondaryShield(true);
        }
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
        playerController.RemovePowerUp();
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

    private void NoWaitRespawnPlayer(PlayerController playerController)
    {
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

    private void MatchTimer()
    {
        if (currentMatchTime > 0)
        {
            currentMatchTime -= Time.deltaTime;
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
            EndMatch();
            ////if we want to disable the players during the win screen not sure exactly what to do here so need some team input
            //foreach (KeyValuePair<GameObject, PlayerController> player in playerDictionary)
            //{
            //    player.Value.enabled = false;
            //}
            //Enumerables.ETeam winningTeam = modeManager.ReturnWinningTeam();
            //Debug.Log(winningTeam);
            //gameMenuUI.gameObject.SetActive(true);
            //winningTeamText.gameObject.SetActive(true);
            //if (winningTeam == ETeam.BlueTeam)
            //{
            //    winningTeamText.text = "Congradulations Blue Team!";
            //    winningTeamText.color = Color.blue;
            //}
            //else if (winningTeam == ETeam.RedTeam)
            //{
            //    winningTeamText.text = "Congradulations Red Team!";
            //    winningTeamText.color = Color.red;
            //}
            //else
            //{
            //    winningTeamText.text = "Draw...";
            //    winningTeamText.color = Color.white;
            //}
            //StartCoroutine(DelayedWinScreen());
        }
    }

    IEnumerator DelayedWinScreen()
    {
        yield return new WaitForSeconds(winScreenWaitTime);
        CharacterSelect();
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
