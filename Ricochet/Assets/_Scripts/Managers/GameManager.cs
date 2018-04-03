using System;
using Enumerables;
using System.Collections;
using System.Collections.Generic;
using CCParticles;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    [Tooltip("Drag the red goal gameObject here")]
    [SerializeField]
    private GameObject redGoal;
    [Tooltip("Drag the blue goal gameObject here")]
    [SerializeField]
    private GameObject blueGoal;
    [Tooltip("Drag the middle column gameObject here")]
    [SerializeField]
    private BoxCollider2D column;
    [Tooltip("Drag the Pointing Arrows Script here")]
    [SerializeField]
    private PointingArrows arrowDisable;
    [Tooltip("Drag the music manager here")]
    [SerializeField]
    private MusicManager musicManager;
    [Tooltip("Drag the sfx manager here")]
    [SerializeField]
    private SFXManager sfxManager;
    [Tooltip("Here be the Game Data me hearties")]
    [SerializeField]
    private GameDataSO gameData;
    [Tooltip("Drag the timer from the UI screen here")]
    [SerializeField]
    private UI_MatchTimer gameTimerText;
    [Tooltip("Drag the neon lights here")]
    [SerializeField]
    private NeonLightController lightsController;
    [Tooltip("Drag the camera here")]
    [SerializeField]
    private MultipleTargetsCamera mtCamera;

    [Header("Events")]
    [SerializeField]
    private GameEvent onGoal;
    [SerializeField]
    private GameEvent onTimerChanged;

    [Header("Timers")]
    [Tooltip("Time before game start")]
    [SerializeField]
    private int matchStartTime = 3;
    [Tooltip("Time after game is over until next scene is loaded")]
    [SerializeField]
    private int timeAfterGameEnd = 3;
    [Tooltip("How long the power up takes to respawn in seconds")]
    [SerializeField]
    private float powerUpRespawnTime = 10f;
    [Tooltip("How long the players take to respawn in seconds")]
    [SerializeField]
    private float ballRespawnTime = 2f;
    [Tooltip("How long a player can hold a ball with CatchNThrow")]
    [SerializeField]
    private float ballHoldTime = 1f;
    [Tooltip("How long a player is invulnerable when they respawn")]
    [SerializeField]
    private float invulnerableTime = 3f;

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
    private List<PlayerController> playerControllers = new List<PlayerController>();
    private List<GameObject> balls = new List<GameObject>();
    private static GameManager instance = null;

    private EMode gameMode;
    private float timeLimit;
    private int scoreLimit = 5;
    private bool gameTimerActive = true;
    private int timeLeftTillStart;
    private bool multiBallInPlay = false;

    // dictionary of players cached based off the GameObject
    private Dictionary<GameObject, PlayerController> playerDictionary = 
        new Dictionary<GameObject, PlayerController>();

    private float currentMatchTime;

    private Color defaultShieldColor;

    public bool GameRunning { get; private set; }

    public float MatchTimeLeft { get { return currentMatchTime; } }
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
            gameTimerText.SetText(currentMatchTime);
        }

        if (powerUpManager)
        {
            defaultShieldColor = powerUpManager.GetPowerUpShieldColor(EPowerUp.None);
        }
        else
        {
            defaultShieldColor = Color.white;
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerController p = go.GetComponent<PlayerController>();
            if (p != null)
            {
                playerControllers.Add(p);
            }
        }
    }

    public void Start()
    {
        StartCoroutine(StartBeginMatchTimer());
        if(gameMode == EMode.Deathmatch)
        {
            ToggleGoals(false);
            if(gameData.GetGameLevel() == BuildIndex.UP_N_OVER_WIDE)
            {
                column.enabled = true;
            }

        }
        else
        {
            if (gameData.GetGameLevel() == BuildIndex.UP_N_OVER_WIDE)
            {
                column.enabled = false;
            }
        }
    }

    #endregion

    #region Getters
    public List<GameObject> GetBallObjects()
    {
        return balls;
    }

    public List<PlayerController> GetPlayers()
    {
        return playerControllers;
    }

    public int GetTimeTillMatchStart()
    {
        return timeLeftTillStart;
    }
    #endregion

    #region Timers
    private IEnumerator StartBeginMatchTimer()
    {
        timeLeftTillStart = matchStartTime;
        gameTimerActive = false;

        foreach (PlayerController p in playerControllers)
        {
            p.DisableMovement(true);
            p.StartingBoost();
        }

        while (timeLeftTillStart > 0)
        {
            onTimerChanged.Raise();
            timeLeftTillStart--;
            sfxManager.PlaySound(soundStorage.GetCountdownSound());
            yield return new WaitForSeconds(1);
        }

        gameTimerActive = true;

        foreach (PlayerController p in playerControllers)
        {
            p.DisableMovement(false);
        }

        StartCoroutine(StartGameTimer());
    }

    private IEnumerator StartGameTimer()
    {
        GameRunning = true;
        currentMatchTime = timeLimit;

        while (currentMatchTime > 0 && GameRunning)
        {
            onTimerChanged.Raise();
            currentMatchTime--;
            if (currentMatchTime <= 4)
                sfxManager.PlaySound(soundStorage.GetCountdownSound());
            yield return new WaitForSeconds(1);
        }

        onTimerChanged.Raise();
        GameRunning = false;

        gameData.SetGameWinner(modeManager.GetMaxScore());
        DeactivatePlayersAndGoals();
        sfxManager.PlaySound(soundStorage.GetMatchEndSound());
        lightsController.HitAllTheLightsAsTeam(modeManager.GetMaxScore(), 10);

        int postMatchTimer = 4;
        while (postMatchTimer > 0)
        {
            postMatchTimer--;
            yield return new WaitForSeconds(1);
        }


        EndMatch();
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

    public void CharacterSelect()
    {
        gameData.ResetGameStatistics();

        LevelSelect.LoadCharacterSelect();
    }

    private void EndMatch()
    {
        LevelSelect.LoadEndGameScene();
    }

    private void ToggleGoals(bool active)
    {
        redGoal.SetActive(active);
        blueGoal.SetActive(active);
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
                    if (!ball.GetTempStatus() && balls.Count <= powerUpManager.GetMaxBalls() - powerUpManager.GetBallSpawnCount())
                    {
                        playerController.PlayPowerupUsedSound();
                        playerController.RemovePowerUp();
                        SpawnMultipleBalls(ball);
                        multiBallInPlay = false;
                    }
                    break;
                case EPowerUp.CatchNThrow:
                    playerController.PlayPowerupUsedSound();
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
                ShieldBurst(playerController);
                playerController.EnableSecondaryShield(false);
                playerController.PlayPowerupUsedSound();
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
                    if(lastTouchedBy.GetTeamNumber() == ETeam.RedTeam)
                    {
                        modeManager.AltUpdateScore(ETeam.BlueTeam, 1);
                    }
                    else
                    {
                        modeManager.AltUpdateScore(ETeam.RedTeam, 1);
                    }
                }
                else
                {
                    if (modeManager.AltUpdateScore(lastTouchedBy.GetTeamNumber(), 1))
                    {
                        SetWinningTeam(lastTouchedBy.GetTeamNumber());
                    }
                }
            }
        }
        else
        {
            if (gameMode == EMode.Deathmatch)
            {
                if (playerController.GetTeamNumber() == ETeam.BlueTeam)
                {
                    if (modeManager.AltUpdateScore(ETeam.RedTeam, 1))
                    {
                        if (playerController.GetTeamNumber() == ETeam.BlueTeam)
                        {
                            SetWinningTeam(ETeam.RedTeam);
                        }
                        else
                        {
                            SetWinningTeam(ETeam.BlueTeam);

                        }
                        BallHandling(ball);
                    }
                }
                else
                {
                    if (modeManager.AltUpdateScore(ETeam.BlueTeam, 1))
                    {
                        if (playerController.GetTeamNumber() == ETeam.BlueTeam)
                        {
                            SetWinningTeam(ETeam.RedTeam);
                        }
                        else
                        {
                            SetWinningTeam(ETeam.BlueTeam);
                        }
                        BallHandling(ball);
                    }
                }
            }
        }

        ball.RedirectBall(collision.relativeVelocity, Vector2.zero);
        playerController.PlayerDead();
        StartCoroutine(RespawnPlayer(playerController));
        StartCoroutine(Camera.main.GetComponent<CCShaders.ChromaticAberrationEffect>().PlayEffect(1));
    }

    private void SetWinningTeam(ETeam team)
    {
        gameData.SetGameWinner(team);
        EndMatch();
    }

    private void BallHandling(Ball affectedBall)
    {
        affectedBall.GetComponent<Ball>().OnBallGoalCollision();
        affectedBall.gameObject.SetActive(false);
        RespawnBall(affectedBall.gameObject);
        NoWaitRespawnAllPlayers();
    }

    public void BallGoalCollision(GameObject ball, ETeam team, int points)
    {
        if (gameMode == EMode.Soccer && GameRunning)
        {
            onGoal.Raise();
            if (lightsController != null)
            {
                ETeam t = team == ETeam.RedTeam ? ETeam.BlueTeam : ETeam.RedTeam;
                lightsController.HitTheTeamLights(t, 3);
            }

            if (!modeManager.UpdateScore(team, points))
            {
                Ball ballScpt = ball.GetComponent<Ball>();
                ballScpt.OnBallGoalCollision();
                ball.SetActive(false);
                RespawnBall(ball);
                NoWaitRespawnAllPlayers();
                
                float multiplier = powerUpManager.GetShrinkMass();
                StartCoroutine(ResetTeamScale(ETeam.RedTeam, 0.0f, multiplier));
                StartCoroutine(ResetTeamScale(ETeam.BlueTeam, 0.0f, multiplier));
            }
            else
            {
                GameRunning = false;

                gameData.SetGameWinner(GetOpposingTeam(team));
                DeactivatePlayersAndGoals();
                lightsController.HitAllTheLightsAsTeam(modeManager.GetMaxScore(), 10);
                StartCoroutine("DelayedWinScreen");
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
        else if (powerUpType == EPowerUp.Multiball)
        {
            multiBallInPlay = true;
        }
        else if (powerUpType == EPowerUp.Shrink)
        {
            ETeam team = playerController.GetTeamNumber();
            ETeam opTeam = GetOpposingTeam(team);

            ShrinkTeam(opTeam);
            playerController.RemovePowerUp();
        }

    }

    public void KillZoneCollision(GameObject haplessSoul)
    {
        if (haplessSoul.activeSelf)
        {
            switch (haplessSoul.tag)
            {
                case "Player":
                    PlayerController playerController;
                    if (!playerDictionary.TryGetValue(haplessSoul, out playerController))
                    {
                        playerController = haplessSoul.GetComponentInParent<PlayerController>();
                        playerDictionary.Add(haplessSoul, playerController);
                    }
                    if (!playerController.IsInvincible())
                    {
                        playerController.PlayerDead();
                        StartCoroutine(RespawnPlayer(playerController));
                    }
                    break;
                case "Ball":
                    //haplessSoul.GetComponent<Ball>().OnBallGoalCollision();
                    //haplessSoul.SetActive(false);
                    //RespawnBall(haplessSoul);
                    haplessSoul.GetComponent<Ball>().ResetPosition();
                    break;
            }
        }
    }

    #endregion

    #region Respawners
    public void LoadPlayer(PlayerController playerController, int playerNumber)
    {
        if(gameData.GetActive(playerNumber))
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
        else
        {
            mtCamera.RemoveTarget(ball.transform);
            Destroy(ball);
        }
    }

    public void RespawnPowerUp(GameObject powerUp)
    {
        StartCoroutine(RespawnPowerUpRoutine(powerUp));
    }

    private void SpawnMultipleBalls(Ball origBall)
    {
        Vector3 tempBallScale = new Vector3(origBall.transform.localScale.x * powerUpManager.GetTempBallScale(), origBall.transform.localScale.y * powerUpManager.GetTempBallScale(), origBall.transform.localScale.z);
        for (int i = 0; i < powerUpManager.GetBallSpawnCount(); i++)
        {
            Ball ball = Instantiate(origBall);
            ball.transform.localScale = tempBallScale;
            ball.SetTempStatus(true);
            mtCamera.AddTarget(ball.transform);
        }
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
        yield return new WaitForSeconds(gameData.playerRespawnTime);

        NoWaitRespawnPlayer(playerController);
    }

    private void NoWaitRespawnPlayer(PlayerController playerController)
    {
        if (playerController.GetCurrentPowerUp() == EPowerUp.Multiball)
        {
            multiBallInPlay = false;
        }
        playerController.RemovePowerUp();
        playerController.gameObject.SetActive(true);
        playerController.SetBallDetection(false);
        if(playerController.GetFrozen())
        {
            playerController.SetFrozen(false);
        }

        StartCoroutine(FlashInvulnerable(invulnerableTime, playerController));

        switch (playerController.GetTeamNumber())
        {
            case ETeam.RedTeam:
                playerController.transform.position = redTeamRespawns[Random.Range(0, redTeamRespawns.Length)].position;
                playerController.transform.rotation = redTeamRespawns[Random.Range(0, redTeamRespawns.Length)].rotation;
                break;
            case ETeam.BlueTeam:
                playerController.transform.position = blueTeamRespawns[Random.Range(0, blueTeamRespawns.Length)].position;
                playerController.transform.rotation = blueTeamRespawns[Random.Range(0, blueTeamRespawns.Length)].rotation;
                break;
        }
    }

    private IEnumerator FlashInvulnerable(float time, PlayerController player)
    {
        float currentTime = 0.0f;
        SpriteRenderer sprite = player.GetSprite();
        float blinkTime = .4f;
        while(currentTime < time)
        {
            sprite.color = new Color(1, 1, 1, .5f);
            yield return new WaitForSeconds(blinkTime);
            sprite.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(blinkTime);
            currentTime += Time.deltaTime + (blinkTime * 2);
        }
        player.SetBallDetection(true);
    }

    public void NoWaitRespawnAllPlayers()
    {
        foreach (PlayerController p in playerControllers)
        {
            if (p.gameObject.activeSelf)
            {
                NoWaitRespawnPlayer(p);
                p.RemovePowerUp();
            }
        }
    }

    private IEnumerator RespawnPowerUpRoutine(GameObject powerUp)
    {
        if (powerUp.GetComponent<PowerUp>().GetPowerUpType() == EPowerUp.Multiball)
        {
            yield return new WaitUntil(() => balls.Count <= powerUpManager.GetMaxBalls() - powerUpManager.GetBallSpawnCount() && !multiBallInPlay);
        }
        yield return new WaitForSeconds(powerUpRespawnTime);
        powerUp.SetActive(true);
    }

    private IEnumerator DelayedWinScreen()
    {
        float postMatchTimer = 4;
        while (postMatchTimer > 0)
        {
            postMatchTimer--;
            yield return new WaitForSeconds(1);
        }
        EndMatch();
    }

    private IEnumerator DropBallCoroutine(PlayerController player, Ball ball)
    {
        yield return new WaitForSeconds(ballHoldTime);

        player.GiveIFrames(0.25f);
        player.SetBallHeld(null);
        ball.SetHeld(false);
        ball.transform.SetParent(null, true);
        ball.RedirectBall(new Vector2(10f, 10f), player.GetRightStick());
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

    #region Public Helpers

    public void FreezePlayer(PlayerController player)
    {
        player.SetFreezeTime(powerUpManager.GetFreezeTime());
    }

    public void FreezePlayers()
    {
        foreach (PlayerController player in playerControllers)
            player.SetFreezeTime(powerUpManager.GetFreezeTime());
    }

    public void PausePlayers(bool pause)
    {
        foreach (PlayerController player in playerControllers)
        {
            player.SetFrozen(pause);
            if (pause)
                player.SetFreezeTime(10f);
            else
                player.SetFreezeTime(-1f);
        }
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

    public AudioClip GetCharacterRespawnSFX(ECharacter character)
    {
        return soundStorage.GetPlayerRespawnSound(character);
    }

    public AudioClip GetCharacterDeathSFX(ECharacter character)
    {
        return soundStorage.GetPlayerDeathSound(character);
    }

    public AudioClip GetCharacterBumpSFX(ECharacter character)
    {
        return soundStorage.GetPlayerBumpSound(character);
    }

    public AudioClip GetScoringSound()
    {
        return soundStorage.GetScoringSound();
    }

    public AudioClip GetBallSound(string tag, bool highVelocity)
    {
        return soundStorage.GetBallSound(tag, highVelocity);
    }
    
    public AudioClip GetPowerupPickupSound(EPowerUp powerUp)
    {
        return soundStorage.GetPowerUpPickUpSound(powerUp);
    }

    public AudioClip GetPowerupUsedSound(EPowerUp powerUp)
    {
        return soundStorage.GetPowerUpUsedSound(powerUp);
    }

    public AudioClip GetMenuClickSound()
    {
        return soundStorage.GetMenuClickSound();
    }

    public AudioClip GetPauseSound()
    {
        return soundStorage.GetPauseSound();
    }

    public AudioClip GetTauntSound(ECharacter character)
    {
        return soundStorage.GetPlayerTauntSound(character);
    }

    #endregion

    #region Private Helpers
    private void ShieldBurst(PlayerController center)
    {
        float radius = powerUpManager.GetBurstRadius();
        float force = powerUpManager.GetBurstForce();
        Vector3 direction;
        Vector2 pushForce;
        int pnum, cnum = center.GetPlayerNumber();

        foreach (PlayerController player in playerControllers)
        {
            pnum = player.GetPlayerNumber();
            if (pnum == cnum || !player.isActiveAndEnabled)
                continue;

            direction = player.transform.position - center.transform.position;
            if (direction.magnitude < radius)
            {
                pushForce = new Vector2(direction.normalized.x, direction.normalized.y);
                pushForce *= force;
                player.GetComponent<Rigidbody2D>().AddForce(pushForce);
            }
        }
        foreach (GameObject ball in balls)
        {
            direction = ball.transform.position - center.transform.position;
            if (direction.magnitude < radius)
            {
                pushForce = new Vector2(direction.normalized.x, direction.normalized.y);
                pushForce *= force;
                ball.GetComponent<Rigidbody2D>().AddForce(pushForce);
            }
        }
    }

    private ETeam GetOpposingTeam(ETeam team)
    {
        ETeam opTeam;
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

    private void ShrinkTeam(ETeam team)
    {
        float scale = powerUpManager.GetShrinkScale();
        float delay = powerUpManager.GetShrinkDuration();
        float multiplier = powerUpManager.GetShrinkMass();

        foreach (PlayerController player in playerControllers)
        {
            if (player.GetTeamNumber() == team)
            {
                player.transform.localScale = new Vector3(scale, scale, scale);
                player.ChangeMomentum(multiplier);
                player.SetIsShrunken(true);
                player.gameObject.GetComponentInChildren<PowerUpParticlesController>().PlayPowerupEffect(EPowerUp.Shrink, 0, true);
            }
        }
        StartCoroutine(ResetTeamScale(team, delay, multiplier));
    }

    IEnumerator ResetTeamScale(ETeam team, float delay, float mult)
    {
        yield return new WaitForSeconds(delay);

        foreach(PlayerController player in playerControllers)
        {
            if (player.GetTeamNumber() == team)
            {
                player.GiveIFrames(powerUpManager.GetIFrames());
                player.transform.localScale = new Vector3(1, 1, 1);
                player.ChangeMomentum(1/mult);
                player.SetIsShrunken(false);
                player.gameObject.GetComponentInChildren<PowerUpParticlesController>().PlayPowerupEffect(EPowerUp.Shrink, 0, false);
            }
        }
    }

    void DeactivatePlayersAndGoals()
    {
        foreach (PlayerController player in playerControllers)
        {
            player.SetAutoJetpack(false);
            player.SetAcceptingInput(false);
        }
        //var blueCollider = blueGoal.GetComponentInChildren<Goal>().gameObject.GetComponent<PolygonCollider2D>() != null
        //    ? blueGoal.GetComponentInChildren<Goal>().gameObject.GetComponent<Collider2D>()
        //    : blueGoal.GetComponentInChildren<Goal>().gameObject.GetComponent<BoxCollider2D>();
        //var redCollider = redGoal.GetComponentInChildren<Goal>().gameObject.GetComponent<PolygonCollider2D>() != null ? 
        try
        {
            blueGoal.GetComponentInChildren<Goal>().gameObject.GetComponent<Collider2D>().enabled = false;
            redGoal.GetComponentInChildren<Goal>().gameObject.GetComponent<Collider2D>().enabled = false;
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e);
        }
    }
    #endregion
}


