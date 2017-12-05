using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enumerables;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    #region Inspector Variables
    [Header("Scene Types")]
    [Tooltip("Whether this scene is a game scene or a menu scene")]
    [SerializeField]
    private bool isGameScene = false;
    [Header("Reference Variables")]
    [Tooltip("Drag the mode manager here")]
    [SerializeField]
    private ModeManager modeManager;
    [Tooltip("Drag the powerup manager here")]
    [SerializeField]
    private PowerUpManager powerUpManager;

    [Tooltip("How long the selected game lasts in seconds")]
    [SerializeField]
    private float gameMatchTime = 120f;
    [Tooltip("Drag the timer from the UI screen here")]
    [SerializeField]
    private Text gameTimerText;

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
    #endregion

    #region Hidden Variables

    private static GameManager instance = null;

    // dictionary of players cached based off the GameObject
    private Dictionary<GameObject, PlayerController> playerDictionary = new Dictionary<GameObject, PlayerController>();

    private float currentMatchTime;
    [SerializeField]
    [Tooltip("Name of level to transition to")]
    private string nextLevel;
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

        if (isGameScene)
        {
            LoadMatchSettings();
            currentMatchTime = gameMatchTime;
            if (gameMatchTime > 0)
            {
                gameTimerText.gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (gameTimerText != null && gameMatchTime > 0)
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
        GameData.ResetGameSetup();
        GameData.ResetGameStatistics();
        SceneManager.LoadSceneAsync(LevelIndex.MAIN_MENU);
    }

    public void CharacterSelect()
    {
        GameData.ResetGameStatistics();
        SceneManager.LoadSceneAsync(LevelIndex.CHARACTER_SELECT);
    }

    public void StartGame()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(LevelIndex.UP_N_OVER);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void EndMatch()
    {        
        if (nextLevel != "")
        {
            SceneManager.LoadSceneAsync(nextLevel);
        }
        else
        {
            SceneManager.LoadSceneAsync("EndGame");
        }
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
            if (playerController == null)
            {
                return;
            }

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

            GameData.gameWinner = GetOpposingTeam(team);
            EndMatch();
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
	public void LoadPlayer(PlayerController playerController, int playerNumber)
	{
		if (GameData.PlayerIsActive (playerNumber)) {
			NoWaitRespawnPlayer (playerController);
		} else {
			playerController.gameObject.SetActive (false);
		}
	}
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

        ball.transform.position = ballRespawns[UnityEngine.Random.Range(0, ballRespawns.Length)].position;

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
                playerController.transform.position = redTeamRespawns[UnityEngine.Random.Range(0, redTeamRespawns.Length)].position;
                playerController.transform.rotation = redTeamRespawns[UnityEngine.Random.Range(0, redTeamRespawns.Length)].rotation;
                break;
            case ETeam.BlueTeam:
                playerController.transform.position = blueTeamRespawns[UnityEngine.Random.Range(0, redTeamRespawns.Length)].position;
                playerController.transform.rotation = blueTeamRespawns[UnityEngine.Random.Range(0, redTeamRespawns.Length)].rotation;
                break;
        }
    }

    private void NoWaitRespawnPlayer(PlayerController playerController)
    {
        playerController.gameObject.SetActive(true);

        switch (playerController.GetTeamNumber())
        {
            case ETeam.RedTeam:
                playerController.transform.position = redTeamRespawns[UnityEngine.Random.Range(0, redTeamRespawns.Length)].position;
                playerController.transform.rotation = redTeamRespawns[UnityEngine.Random.Range(0, redTeamRespawns.Length)].rotation;
                break;
            case ETeam.BlueTeam:
                playerController.transform.position = blueTeamRespawns[UnityEngine.Random.Range(0, blueTeamRespawns.Length)].position;
                playerController.transform.rotation = blueTeamRespawns[UnityEngine.Random.Range(0, blueTeamRespawns.Length)].rotation;
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

    #region Private Helpers
    private ETeam GetOpposingTeam(ETeam team)
    {
        var opTeam = ETeam.None;
        if(team == ETeam.BlueTeam)
        {
            opTeam = ETeam.RedTeam;
        }
        else
        {
            opTeam = ETeam.BlueTeam;
        }
        return opTeam;
    }

    private void LoadMatchSettings()
    {
        //Both match limit settings are 0. Set inspector values for both.
        if (GameData.matchScoreLimit == 0 && GameData.matchTimeLimit == 0)
        {
            GameData.matchScoreLimit = scoreLimit;
            GameData.matchTimeLimit = gameMatchTime;
        }
        else
        {
            scoreLimit = GameData.matchScoreLimit;
            gameMatchTime = GameData.matchTimeLimit;
        }
    }
    #endregion
}


