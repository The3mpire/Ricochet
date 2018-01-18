﻿using Enumerables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [Tooltip("Drag the sound storage here")]
    [SerializeField]
    private SoundStorage soundStorage;
    [Tooltip("Drag the music manager here")]
    [SerializeField]
    private MusicManager musicManager;
    [SerializeField]
    private GameDataSO gameData;

    [Header("Game Match Variables")]
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
    #endregion

    #region Hidden Variables
    private List<GameObject> balls = new List<GameObject>();
    private static GameManager instance = null;

    private EMode gameMode;
    private float timeLimit;
    private int scoreLimit = 5;

    // dictionary of players cached based off the GameObject
    private Dictionary<GameObject, PlayerController> playerDictionary = new Dictionary<GameObject, PlayerController>();

    private float currentMatchTime;

    private Color defaultShieldColor;
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

        LoadMatchSettings();

        currentMatchTime = timeLimit;
        if (timeLimit > 0)
        {
            gameTimerText.gameObject.SetActive(true);
        }

        if (powerUpManager)
        {
            defaultShieldColor = powerUpManager.GetPowerUpShieldColor(EPowerUp.None);
        }
        else
        {
            defaultShieldColor = Color.white;
        }
    }

    void Update()
    {
        if (gameTimerText != null && timeLimit > 0)
        {
            MatchTimer();
        }
    }
    #endregion

    #region Getters
    public List<GameObject> GetBallObjects()
    {
        return balls;
    }

    public GameObject[] GetPlayerObjects()
    {
        return players;
    }
    #endregion

    public void AddBallObject(GameObject ball)
    {
        balls.Add(ball);
    }

    public void RemoveBallObject(GameObject ball)
    {
        balls.Remove(ball);
    }

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
        gameData.ResetGameStatistics();
        LevelSelect.LoadMainMenu();
    }

    public void CharacterSelect()
    {
        gameData.ResetGameStatistics();
        LevelSelect.LoadCharacterSelect();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void EndMatch()
    {
        LevelSelect.LoadEndGameScene();
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

        playerController.Rumble();

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
        //Secondary Shield used to use shield script to get player but things have changed with that
        //so we are going to need to figure out how to do this if we don't want to use get parent
        PlayerController playerController = secondaryShield.GetComponentInParent<PlayerController>();
        switch (playerController.GetCurrentPowerUp())
        {
            case EPowerUp.CircleShield:
                playerController.EnableSecondaryShield(false);
                playerController.RemovePowerUp();
                break;
        }
    }

    public void BallPlayerCollision(GameObject player, Collision2D collision)
    {
        PlayerController playerController;
        if (!playerDictionary.TryGetValue(player, out playerController))
        {
            playerController = player.GetComponent<PlayerController>();
            playerDictionary.Add(player, playerController);
        }

        Ball ball = collision.gameObject.GetComponent<Ball>();

        // Check if the ball has been touched by anyone
        PlayerController lastTouchedBy = ball.GetLastTouchedBy(playerController);
        if (lastTouchedBy != null)
        {
            lastTouchedBy.RegisterKill(playerController);

            if (gameMode == EMode.Deathmatch)
            {
                if (lastTouchedBy.GetTeamNumber() == playerController.GetTeamNumber())
                {
                    modeManager.AltUpdateScore(lastTouchedBy.GetTeamNumber(), -1);
                }
                else
                {
                    if (modeManager.AltUpdateScore(lastTouchedBy.GetTeamNumber(), 1))
                    {
                        gameData.SetGameWinner(lastTouchedBy.GetTeamNumber());
                        EndMatch();
                    }
                }
            }
        }
        else
        {
            if (gameMode == EMode.Deathmatch)
            {
                if (modeManager.AltUpdateScore(playerController.GetTeamNumber(), -1))
                {
                    if (playerController.GetTeamNumber() == ETeam.BlueTeam)
                    {
                        gameData.SetGameWinner(ETeam.RedTeam);
                        EndMatch();
                    }
                    else
                    {
                        gameData.SetGameWinner(ETeam.BlueTeam);
                        EndMatch();
                    }
                    ball.GetComponent<Ball>().OnBallGoalCollision();
                    ball.gameObject.SetActive(false);
                    RespawnBall(ball.gameObject);
                    NoWaitRespawnAllPlayers();
                }
            }
        }

        ball.RedirectBall(collision.relativeVelocity);
        playerController.PlayerDead();
        StartCoroutine(RespawnPlayer(playerController));
    }

    public void BallGoalCollision(GameObject ball, ETeam team, int points)
    {
        if (gameMode == EMode.Soccer)
        {
            if (!modeManager.UpdateScore(team, points))
            {
                ball.GetComponent<Ball>().OnBallGoalCollision();
                ball.SetActive(false);
                RespawnBall(ball);
                NoWaitRespawnAllPlayers();
            }
            else
            {
                gameData.SetGameWinner(GetOpposingTeam(team));
                EndMatch();
            }
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
        if (playerNumber <= gameData.GetPlayerCount())
        {
            NoWaitRespawnPlayer(playerController);
        }
        else
        {
            playerController.gameObject.SetActive(false);
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
        ball.transform.position = ballRespawns[Random.Range(0, ballRespawns.Length)].position;

        ball.SetActive(true);
        ball.GetComponent<Ball>().SetBeenHit(false);
    }

    private IEnumerator RespawnPlayer(PlayerController playerController)
    {
        yield return new WaitForSeconds(playerRespawnTime);

        NoWaitRespawnPlayer(playerController);
    }

    private void NoWaitRespawnPlayer(PlayerController playerController)
    {
        playerController.RemovePowerUp();
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

    public void NoWaitRespawnAllPlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            PlayerController currentPlayer = players[i].GetComponent<PlayerController>();
            if (players[i].activeSelf)
            {
                NoWaitRespawnPlayer(currentPlayer);
                currentPlayer.RemovePowerUp();
            }
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
            gameData.SetGameWinner(modeManager.GetMaxScore());
            timeLimit = 0.0f;
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

    #region Sound
    public void SetMusicVolume(float vol = .8f)
    {
        musicManager.SetMusicVolume(vol);
    }

    public float GetMusicVolume()
    {
        return musicManager.GetMusicVolume();
    }

    /// <param name="volume"> scales the SFX volume (NOT the overall volume)</param>
    public AudioClip GetCharacterSFX(ECharacter character, ECharacterAction movement)
    {
        switch (movement)
        {
            case ECharacterAction.Dash:
                return soundStorage.GetPlayerDashSound(character);
            case ECharacterAction.Death:
                return soundStorage.GetPlayerDeathSound(character);
            default: //ECharacterMovement.Jetpack:
                return soundStorage.GetPlayerJetpackSound(character);
        }
    }

    public AudioClip GetScoringSound()
    {
        return soundStorage.GetScoringSound();
    }

    public AudioClip GetBallSound()
    {
        return soundStorage.GetBallSound();
    }

    public AudioClip GetMenuClickSound()
    {
        return soundStorage.GetMenuClickSound();
    }
    #endregion

    #region Private Helpers
    private ETeam GetOpposingTeam(ETeam team)
    {
        var opTeam = ETeam.None;
        if (team == ETeam.BlueTeam)
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
        gameData.SetGameLevel((BuildIndex)SceneManager.GetActiveScene().buildIndex);
        scoreLimit = gameData.GetScoreLimit();
        timeLimit = gameData.GetTimeLimit();
        gameMode = gameData.GetGameMode();
    }
    #endregion
}


