using System.Collections;
using System.Collections.Generic;
using Enumerables;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Instance Variables
    [SerializeField]
    private GameDataSO gameData;

    [Header("Player Settings")]
    [Tooltip("Which player this is")]
    [SerializeField]
    private int playerNumber = 1;
    [Tooltip("Which team the player is on")]
    [SerializeField]
    private ETeam team;
    [Tooltip("Autojetpack?")]
    [SerializeField]
    private bool autoJetpack = true;

    [Header("Movement Settings")]
    [Tooltip("Gravity scale on player")]
    [SerializeField]
    private float gravScale = 8f;
    [Tooltip("How frequently the player sprite should blink on death")]
    [SerializeField]
    public float blinkMultiplier = 0.2f;
    [Tooltip("Acceleration constant while moving laterally")]
    [SerializeField]
    private float lateralAcceleration;
    [Tooltip("Max lateral speed when falling")]
    [SerializeField]
    private float fallingLateralSpeed = 1f;

    [Header("Standard movement")]
    [Tooltip("Acceleration/deceleartion constant while using jetpack")]
    [SerializeField]
    private float thrusterAcceleration = 0.05f;
    [Tooltip("Max speed while using jetpack w/ no directional input")]
    [SerializeField]
    private float upThrusterSpeed = 5f;
    [Tooltip("Move speed while using jetpack w/ directional input")]
    [SerializeField]
    private float thrusterSpeed = 15f;
    [Tooltip("How much faster is downward movement? 1.0 is the same. > 1 is faster")]
    [SerializeField]
    private float downwardMovementSpeedup = 1.3f;

    [Header("Precision Movement")]
    [Tooltip("Acceleration/deceleartion constant while using precision movement")]
    [SerializeField]
    private float precisionThrusterAcceleration = 0.05f;
    [Tooltip("Move speed while using precision movement w/ directional input")]
    [SerializeField]
    private float precisionThrusterSpeed = 15f;

    [Header("Other movement")]
    [Tooltip("Move speed while in the air, not using jetpack")]
    [SerializeField]
    private float airMoveSpeed = 5f;
    [Tooltip("Move speed while grounded")]
    [SerializeField]
    private float groundedMoveSpeed = 12.5f;
    [Tooltip("How quickly the player can change direction. 0 = instantaneously, 1 = normally")]
    [SerializeField]
    private float directionSwitchRatio = 1f;
    [Tooltip("How much bounce off two opposite players will have as a percentage")]
    [SerializeField]
    private float boingFactor = 1.0f;
    [Tooltip("gravity scale for the rigid body so when the player unfreezes we can reset the gravity scale")]
    [SerializeField]
    private float gravityScale = 1.0f;

    [Header("Controller Settings")]
    [Tooltip("The motor to be used (default is 0)")]
    [SerializeField]
    private int motorIndex;
    [Tooltip("Rumble intensity")]
    [SerializeField]
    private float motorLevel = .5f;
    [Tooltip("How long the rumble should last")]
    [SerializeField]
    private float rumbleDuration = .1f;
    [Tooltip("How much the above values should be multiplied by on death")]
    [SerializeField]
    private float rumbleMultiplier = 2f;

    [Header("Reference Components")]
    [Tooltip("The Shield Transform")]
    [SerializeField]
    private Transform shieldTransform;
    [Tooltip("The Shield")]
    [SerializeField]
    private GameObject shieldObject;
    [Tooltip("Drag the SpriteRenderer here")]
    [SerializeField]
    private SpriteRenderer sprite;
    [Tooltip("Drag the powerup particle here")]
    [SerializeField]
    private CCParticles.PowerUpParticlesController powerupParticleController;
    [Tooltip("Drag the Tag canvas's tag object here")]
    [SerializeField]
    private Text playerNumberTag;
    [Tooltip("Drag the player's rigidbody here")]
    [SerializeField]
    private Rigidbody2D rigid;
    [Tooltip("Drag the player's \"groundCheck\" here")]
    [SerializeField]
    private Transform groundCheck;
    [Tooltip("Drag the player's audio source here")]
    [SerializeField]
    private AudioSource audioSource;
    [Tooltip("Freeze Color")]
    [SerializeField]
    private Color freezeColor;
    [Tooltip("The ball detection collider object")]
    [SerializeField]
    private GameObject detectionColliderObject;

    [Header("Secondary Items")]
    [Tooltip("Drag the player's power up circle shield here")]
    [SerializeField]
    private GameObject circleShield;
    #endregion

    #region Hidden Variables
    private GameManager gameManagerInstance;
    private Shield shield;

    private Ball ballHeld;

    private Animator animator;
    private bool flip;
    private ParticleSystem jetpackParticle;

    private ECharacter chosenCharacter;
    private EPowerUp currPowerUp = EPowerUp.None;
    private Player player;
    private List<PlayerController> killList;

    private bool hasPowerUp;
    private bool isInvincible;
    private bool isFrozen;
    private bool isShrunken;
    private float remainingFreezeTime;
    
    private float powerUpTimer;
    private Vector2 previousVelocity;

    private float leftStickHorz;
    private float leftStickVert;
    private float rightStickHorz;
    private float rightStickVert;
    private float leftTriggerAxis;

    private bool movementDisabled;
    private bool acceptingInput;

    private PlayerDashController dashController;
    #endregion

    #region Monobehaviour
    private void Awake()
    {
        if (playerNumberTag)
        {
            playerNumberTag.text = playerNumber.ToString();
        }
        //powerupParticle.Stop();
        isFrozen = false;
        isShrunken = false;

        killList = new List<PlayerController>();
        rigid.gravityScale = 0;
        rightStickHorz = 1;
        rightStickVert = 0;
        shield = GetComponentInChildren<Shield>();
        animator = transform.GetComponentInChildren<Animator>();
        team = gameData.GetPlayerTeam(playerNumber - 1);
        chosenCharacter = gameData.GetPlayerCharacter(playerNumber - 1);
        shield.SetTeamColor(team);
        acceptingInput = true;

        dashController = GetComponent<PlayerDashController>();

        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            gameManagerInstance.LoadPlayer(this, playerNumber);
        }
    }

    private void Start()
    {
        player = ReInput.players.GetPlayer(playerNumber - 1);
        sprite.color = PlayerColorData.getColor(playerNumber, team);
        transform.GetComponentInChildren<BasePlayerSetup>().SetupCharacter(chosenCharacter, 0);
        audioSource.PlayOneShot(gameManagerInstance.GetCharacterRespawnSFX(chosenCharacter));
    }

    private void Update()
    {
        if (acceptingInput)
        {
            leftStickHorz = player.GetAxis("MoveHorizontal");
            leftStickVert = player.GetAxis("MoveVertical");
            leftTriggerAxis = player.GetAxis("Jetpack");

            if (player.GetAxis("RightStickHorizontal") != 0)
            {
                rightStickHorz = player.GetAxis("RightStickHorizontal");
            }
            if (player.GetAxis("RightStickVertical") != 0)
            {
                rightStickVert = player.GetAxis("RightStickVertical");
            }
        }
        else
        {
            leftStickHorz = 0;
            leftStickVert = 0;
            leftTriggerAxis = 0;
        }

        if (player.GetAxis("Taunt") != 0)
        {
            animator.SetBool("isTaunting", true);
            if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
            {
                if (!audioSource.isPlaying)
                {
                    ECharacter character = gameData.GetPlayerCharacter(playerNumber - 1);
                    audioSource.PlayOneShot(gameManagerInstance.GetTauntSound(character));
                }
            }
        }
        else
        {
            animator.SetBool("isTaunting", false);
        }


        
        if (remainingFreezeTime > 0)
        {
            rigid.velocity = new Vector3(0, 0, 0);
            remainingFreezeTime -= Time.deltaTime;
            if (remainingFreezeTime <= 0)
            {
                isFrozen = false;
                sprite.color = Color.white;
            }
        }

        if (!isFrozen)
        {
            RotateShield();
            if (!movementDisabled)
            {
                Flip();
            }
        }
    }

    private void FixedUpdate()
    {
        previousVelocity = rigid.velocity;
        if (!movementDisabled && !isFrozen)
        {
            Move();
        }

        HandleAnimator();
    }
    #endregion

    #region Collision Management
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInvincible)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ball") && collision.otherCollider.CompareTag("Player"))
            {
                if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
                {
                    gameManagerInstance.BallPlayerCollision(gameObject, collision);
                }
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
                {
                    PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
                    if (team != otherPlayer.team)
                    {
                        EPowerUp otherPlayerPowUp = otherPlayer.GetCurrentPowerUp();
                        if(otherPlayerPowUp == EPowerUp.Freeze)
                        {
                            powerupParticleController.PlayPowerupEffect(EPowerUp.Freeze, 1, true);
                            audioSource.PlayOneShot(gameManagerInstance.GetPowerupUsedSound(EPowerUp.Freeze));
                            isFrozen = true;
                            rigid.gravityScale = 0.0f;
                            rigid.velocity = new Vector3(0, 0, 0);
                            sprite.color = freezeColor;
                            otherPlayer.RemovePowerUp();
                            gameManagerInstance.FreezePlayer(this);
                            if(remainingFreezeTime <= 0)
                            {
                                isFrozen = false;
                                powerupParticleController.PlayPowerupEffect(EPowerUp.Freeze, 1, false);
                            }
                        }
                        if (!isShrunken)
                        {
                            audioSource.PlayOneShot(gameManagerInstance.GetCharacterBumpSFX(chosenCharacter));
                            Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();
                            body.velocity = otherPlayer.GetPreviousVelocity() * -boingFactor;
                        }
                        Rumble(1.25f);
                    }
                }
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
                {
                    audioSource.PlayOneShot(gameManagerInstance.GetCharacterBumpSFX(chosenCharacter), 1f);
                }
            }
        }
    }

    #endregion

    #region Helpers
    private void HandleAnimator()
    {
        bool grounded = IsGrounded();
        if (!grounded)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);

            if (rigid.velocity != Vector2.zero)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                if (chosenCharacter == ECharacter.Computer && sprite.flipX)
                {
                    sprite.flipX = flip;
                }
                animator.SetBool("isWalking", false);
            }
        }

        if (rigid.velocity.magnitude > thrusterSpeed+15)
        {
            animator.SetTrigger("dash");
        }
    }

    private void Move()
    {
        Vector2 moveDirection;
        bool jetpacking = autoJetpack ? leftTriggerAxis == 0 : leftTriggerAxis != 0;
        bool precisionMode = autoJetpack ? leftTriggerAxis != 0 : false;

        if (jetpacking)
        {
            if (IsGrounded())
            {
                // walking

                moveDirection = new Vector2(leftStickHorz, leftStickVert);
                if (jetpackParticle && jetpackParticle.isPlaying)
                {
                    jetpackParticle.Stop();
                }
                float x = 0;
                if (moveDirection.x > 0)
                {
                    x = rigid.velocity.x > 0 ? rigid.velocity.x : rigid.velocity.x * (directionSwitchRatio - 0.2f);
                    x = Mathf.Min(x + (moveDirection.x * 2.5f), groundedMoveSpeed * Mathf.Abs(moveDirection.x));
                }
                else if (moveDirection.x < 0)
                {
                    x = rigid.velocity.x < 0 ? rigid.velocity.x : rigid.velocity.x * (directionSwitchRatio - 0.2f);
                    x = Mathf.Max(x + (moveDirection.x * 2.5f), -groundedMoveSpeed * Mathf.Abs(moveDirection.x));
                }
                else
                {
                    float sign = rigid.velocity.x / Mathf.Abs(rigid.velocity.x);
                    x = Mathf.Abs(rigid.velocity.x) > 1.5f ? rigid.velocity.x - (sign * 1.2f) : 0f;
                }
                float y = moveDirection.y >= 0 ? Mathf.Min(Mathf.Max(rigid.velocity.y, rigid.velocity.y * directionSwitchRatio) + (moveDirection.y * thrusterAcceleration), thrusterSpeed + (rigid.velocity.y - thrusterSpeed) * 0.85f) :
                        Mathf.Max(Mathf.Min(rigid.velocity.y, rigid.velocity.y * directionSwitchRatio) + (moveDirection.y * thrusterAcceleration * downwardMovementSpeedup), -(thrusterSpeed * downwardMovementSpeedup) + (rigid.velocity.y + (thrusterSpeed * downwardMovementSpeedup)) * 0.85f);
                rigid.velocity = new Vector2(x, y);
            }
            else
            {
                // flying
                if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
                {
                    //TODO come back to this with a coroutine?
                    //Debug.Log("we jetting");
                    //audioSource.PlayOneShot(gameManagerInstance.GetCharacterSFX(chosenCharacter, ECharacterAction.Jetpack));
                }

                moveDirection = new Vector2(leftStickHorz, leftStickVert).normalized;
                if (jetpackParticle && !jetpackParticle.isPlaying && !isFrozen)
                {
                    jetpackParticle.Play();
                }
                // If there is no directional input, decelerate movement to a still hover
                if (moveDirection == Vector2.zero)
                {
                    float x = Mathf.Abs(rigid.velocity.x) > 0.1 ? rigid.velocity.x * 0.8f : 0f;
                    float y = Mathf.Abs(rigid.velocity.y) > 0.5f ? rigid.velocity.y * 0.90f : 0;
                    rigid.velocity = new Vector2(x, y);
                } // else go in the direction of input with thruster speed
                else
                {
                    float lta = autoJetpack ? 1 : leftTriggerAxis;
                    float ts = thrusterSpeed * lta;
                    float x, y;
                    x = moveDirection.x > 0 ? Mathf.Min(Mathf.Max(rigid.velocity.x, rigid.velocity.x * directionSwitchRatio) + (moveDirection.x * thrusterAcceleration * lta), ts + (rigid.velocity.x - ts) * 0.85f) :
                        Mathf.Max(Mathf.Min(rigid.velocity.x, rigid.velocity.x * directionSwitchRatio) + (moveDirection.x * thrusterAcceleration * lta), -ts + (rigid.velocity.x + ts) * 0.85f);

                    y = moveDirection.y >= 0 ? Mathf.Min(Mathf.Max(rigid.velocity.y, rigid.velocity.y * directionSwitchRatio) + (moveDirection.y * thrusterAcceleration * lta), ts + (rigid.velocity.y - ts) * 0.85f) :
                        Mathf.Max(Mathf.Min(rigid.velocity.y, rigid.velocity.y * directionSwitchRatio) + (moveDirection.y * thrusterAcceleration * downwardMovementSpeedup * lta), -(ts * downwardMovementSpeedup) + (rigid.velocity.y + (ts * downwardMovementSpeedup)) * 0.85f);

                    rigid.velocity = new Vector2(x, y);
                }
            }
        }
        else if (precisionMode)
        { // if precision mode is engaged, Move at reduces speed and acceleration
            moveDirection = new Vector2(leftStickHorz, leftStickVert).normalized;
            if (jetpackParticle)
            {
                jetpackParticle.Stop();
            }
            if (player.GetAxisRawPrev("Jetpack") == 0)
            {
                ECharacter character = gameData.GetPlayerCharacter(playerNumber - 1);
                audioSource.PlayOneShot(gameManagerInstance.GetCharacterSFX(character,ECharacterAction.Jetpack));
            }
            else
            {
                if (jetpackParticle && !jetpackParticle.isPlaying && !isFrozen)
                {
                    jetpackParticle.Play();
                }
            }
            // If there is no directional input, decelerate movement to a still hover
            if (moveDirection == Vector2.zero)
            {
                float x = Mathf.Abs(rigid.velocity.x) > 0.1 ? rigid.velocity.x * 0.8f : 0f;
                float y = Mathf.Abs(rigid.velocity.y) > 0.5f ? rigid.velocity.y * 0.9f : 0;
                rigid.velocity = new Vector2(x, y);
            } // else go in the direction of input with thruster speed
            else
            {
                float x, y;
                x = moveDirection.x > 0 ? Mathf.Min(Mathf.Max(rigid.velocity.x, rigid.velocity.x * directionSwitchRatio) + (moveDirection.x * precisionThrusterAcceleration), precisionThrusterSpeed + (rigid.velocity.x - precisionThrusterSpeed) * 0.85f) :
                    Mathf.Max(Mathf.Min(rigid.velocity.x, rigid.velocity.x * directionSwitchRatio) + (moveDirection.x * precisionThrusterAcceleration), -precisionThrusterSpeed + (rigid.velocity.x + precisionThrusterSpeed) * 0.85f);

                y = moveDirection.y >= 0 ? Mathf.Min(Mathf.Max(rigid.velocity.y, rigid.velocity.y * directionSwitchRatio) + (moveDirection.y * precisionThrusterAcceleration), precisionThrusterSpeed + (rigid.velocity.y - precisionThrusterSpeed) * 0.85f) :
                    Mathf.Max(Mathf.Min(rigid.velocity.y, rigid.velocity.y * directionSwitchRatio) + (moveDirection.y * precisionThrusterAcceleration), -precisionThrusterSpeed + (rigid.velocity.y + precisionThrusterSpeed) * 0.85f);

                rigid.velocity = new Vector2(x, y);
            }
        }
        else if (!jetpacking && !autoJetpack)
        {
            moveDirection = new Vector2(leftStickHorz, 0);
            if (this.jetpackParticle)
            {
                this.jetpackParticle.Stop();
            }

            if (IsGrounded())
            {
                float x = 0;
                if (moveDirection.x > 0)
                {
                    x = rigid.velocity.x > 0 ? rigid.velocity.x : rigid.velocity.x * (directionSwitchRatio - 0.2f);
                    x = Mathf.Min(x + (moveDirection.x * 2.5f), groundedMoveSpeed * Mathf.Abs(moveDirection.x));
                }
                else if (moveDirection.x < 0)
                {
                    x = rigid.velocity.x < 0 ? rigid.velocity.x : rigid.velocity.x * (directionSwitchRatio - 0.2f);
                    x = Mathf.Max(x + (moveDirection.x * 2.5f), -groundedMoveSpeed * Mathf.Abs(moveDirection.x));
                }
                else
                {
                    float sign = rigid.velocity.x / Mathf.Abs(rigid.velocity.x);
                    x = Mathf.Abs(rigid.velocity.x) > 1.5f ? rigid.velocity.x - (sign * 1.2f) : 0f;
                }
                rigid.velocity = new Vector2(x, 0f);
            }
            else
            {
                float x = 0, y = 0;
                if (rigid.velocity.x > fallingLateralSpeed)
                {
                    x = rigid.velocity.x - (rigid.velocity.x - fallingLateralSpeed) * 0.1f;
                }
                else if (rigid.velocity.x < -fallingLateralSpeed)
                {
                    x = rigid.velocity.x - ((rigid.velocity.x + fallingLateralSpeed) * 0.1f);
                }

                if (moveDirection.x > 0)
                {
                    x = Mathf.Min(rigid.velocity.x + (moveDirection.x * lateralAcceleration), rigid.velocity.x - (rigid.velocity.x - fallingLateralSpeed) * 0.1f);
                }
                else if (moveDirection.x < 0)
                {
                    x = Mathf.Max(rigid.velocity.x + (moveDirection.x * lateralAcceleration), rigid.velocity.x - ((rigid.velocity.x + fallingLateralSpeed) * 0.1f));
                }

                y = Mathf.Max(rigid.velocity.y - 0.75f, -airMoveSpeed);
                rigid.velocity = new Vector2(x, y);
            }
        }
    }

    private void Flip()
    {
        if (leftStickHorz < 0)
        {
            sprite.flipX = flip;
        }
        else if (leftStickHorz > 0)
        {
            sprite.flipX = !flip;
        }
    }

    private void RotateShield()
    {
        //make sure there is magnitude
        if ((Mathf.Abs(rightStickHorz) > 0 || Mathf.Abs(rightStickVert) > 0) && shield.gameObject.activeSelf && !isFrozen)
        {

            if (rightStickHorz > 0)
            {
                shieldTransform.localRotation = Quaternion.Euler(new Vector3(shieldTransform.localRotation.eulerAngles.x, shieldTransform.localRotation.eulerAngles.y, -Vector2.Angle(new Vector2(rightStickHorz, -rightStickVert), Vector2.down) + 90));
            }
            else
            {
                shieldTransform.localRotation = Quaternion.Euler(new Vector3(shieldTransform.localRotation.eulerAngles.x, shieldTransform.localRotation.eulerAngles.y, Vector2.Angle(new Vector2(rightStickHorz, -rightStickVert), Vector2.down) + 90));
            }
            if (ballHeld != null)
            {
                if (rightStickHorz > 0)
                {
                    ballHeld.transform.localRotation = Quaternion.Euler(new Vector3(ballHeld.transform.localRotation.eulerAngles.x, ballHeld.transform.localRotation.eulerAngles.y, -Vector2.Angle(new Vector2(rightStickHorz, -rightStickVert), Vector2.down) + 90));
                }
                else
                {
                    ballHeld.transform.localRotation = Quaternion.Euler(new Vector3(ballHeld.transform.localRotation.eulerAngles.x, ballHeld.transform.localRotation.eulerAngles.y, Vector2.Angle(new Vector2(rightStickHorz, -rightStickVert), Vector2.down) + 90));
                }
            }
        }
    }

    private IEnumerator DelayedEndIFrames(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isInvincible = false;
    }
    #endregion

    #region External Functions
    public void Rumble(float multiplier = 1f)
    {
        player.SetVibration(motorIndex, motorLevel * multiplier, rumbleDuration * multiplier);
    }

    public void ReceivePowerUp(EPowerUp powerUp, Color powerUpColor)
    {
        if (powerupParticleController)
        {
            powerupParticleController.PlayPowerupEffect(powerUp, 0, true);
        }
        audioSource.PlayOneShot(gameManagerInstance.GetPowerupPickupSound(powerUp));
        hasPowerUp = true;
        currPowerUp = powerUp;
    }
    public void GiveIFrames(float seconds)
    {
        if (gameObject.activeSelf)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            StartCoroutine(DelayedEndIFrames(seconds));
        }
    }
    public void ChangeMomentum(float m)
    {
        rigid.mass *= m;
    }

    public void AddVelocity(Vector2 velocity)
    {
        rigid.velocity += velocity;
    }

    public void RemovePowerUp()
    {
        //if (powerupParticle && powerupParticle.isPlaying)
        //{
        //    powerupParticle.Stop();
        //}
        if (powerupParticleController)
        {
            powerupParticleController.StopPowerupEffect(currPowerUp, 0);
        }
        hasPowerUp = false;
        EnableSecondaryShield(false);

        currPowerUp = EPowerUp.None;
    }

    public void PlayPowerupUsedSound()
    {
        audioSource.PlayOneShot(gameManagerInstance.GetPowerupUsedSound(currPowerUp));
    }

    public void PlayPauseSound()
    {
        audioSource.PlayOneShot(gameManagerInstance.GetPauseSound(true));
    }

    public void RegisterKill(PlayerController otherPlayer)
    {
        killList.Add(otherPlayer);
    }

    public void PlayerDead()
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            audioSource.PlayOneShot(gameManagerInstance.GetCharacterSFX(chosenCharacter, ECharacterAction.Death));
        }
        Rumble(rumbleMultiplier);
        
        rigid.velocity = Vector3.zero;
        detectionColliderObject.SetActive(false);
        StartCoroutine(Blink(gameData.playerRespawnTime));
        StartCoroutine(KillPlayer());
    }

    public void StartingBoost()
    {
        if (gameObject.activeSelf)
        {
            dashController.StartingBoost();
        }
    }

    private IEnumerator Blink(float waitTime)
    {
        float endTime = Time.time + waitTime;
        while (Time.time < endTime)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(blinkMultiplier);
            sprite.enabled = true;
            yield return new WaitForSeconds(blinkMultiplier);
        }
    }

    private IEnumerator KillPlayer()
    {
        if(chosenCharacter == ECharacter.Computer && sprite.flipX)
        {
            sprite.flipX = flip;
        }
        this.animator.SetBool("isDead", true);
        isInvincible = true;
        movementDisabled = true;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            audioSource.PlayOneShot(gameManagerInstance.GetCharacterDeathSFX(chosenCharacter)); 
        }

        shield.gameObject.SetActive(false);
        yield return new WaitForSeconds(gameData.playerRespawnTime);
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        this.animator.SetBool("isDead", false);
        isInvincible = false;
        detectionColliderObject.SetActive(true);
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        shield.gameObject.SetActive(true);
        dashController.ResetDashController();
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            audioSource.PlayOneShot(gameManagerInstance.GetCharacterRespawnSFX(chosenCharacter)); 
        }
        movementDisabled = false;
    }

    public void EnableSecondaryShield(bool status)
    {
        switch (currPowerUp)
        {
            case EPowerUp.CircleShield:
                circleShield.SetActive(status);
                break;
        }
    }
    #endregion

    #region Getters and Setters
    public bool IsInvincible()
    {
        return isInvincible;
    }
    public bool IsGrounded()
    {
        Vector2 groundChecker = new Vector2(groundCheck.position.x, groundCheck.position.y - 0.1f);

        bool touchingGround = Physics2D.Linecast(transform.position, groundChecker, 1 << LayerMask.NameToLayer("Ground"));
        touchingGround = autoJetpack ? touchingGround : (touchingGround && leftTriggerAxis == 0);

        return touchingGround;
    }

    public Vector2 GetShieldDirection()
    {
        return shieldTransform.right.normalized;
    }

    public Player GetPlayer()
    {
        return ReInput.players.GetPlayer(playerNumber - 1);
    }

    public ECharacter GetCharacter()
    {
        return chosenCharacter;
    }

    public bool GetAutoJetpack()
    {
        return autoJetpack;
    }

    public void SetAutoJetpack(bool aj)
    {
        autoJetpack = aj;
    }

    public void SetAcceptingInput(bool ai)
    {
        acceptingInput = ai;
    }

    public void DisableMovement(bool movementDisabled)
    {
        this.movementDisabled = movementDisabled;
    }

    public bool MovementDisabled()
    {
        return movementDisabled;
    }

    public Transform GetShieldTransform()
    {
        return shieldTransform;
    }

    public float GetLeftTrigger()
    {
        return leftTriggerAxis;
    }

    public Vector2 GetLeftStick()
    {
        return new Vector2(leftStickHorz, leftStickVert);
    }

    public Vector2 GetNormalizedLeftStick()
    {
        Vector2 vec = new Vector2(leftStickHorz, leftStickVert).normalized;
        if (vec.magnitude == 0)
        {
            vec = GetPreviousVelocity().normalized;
        }
        return vec;
    }

    public Vector2 GetRightStick()
    {
        return new Vector2(rightStickHorz, rightStickVert);
    }

    public void SetBallHeld(Ball ball)
    {
        ballHeld = ball;
    }

    public EPowerUp GetCurrentPowerUp()
    {
        return currPowerUp;
    }

    public ETeam GetTeamNumber()
    {
        return team;
    }

    public void SetBodyType(Sprite image)
    {
        sprite.sprite = image;
    }

    public void SetBodyScale(float scaleSize)
    {
        sprite.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
    }

    public Vector2 GetPreviousVelocity()
    {
        return previousVelocity;
    }

    public int GetPlayerNumber()
    {
        return playerNumber;
    }

    public void SetPlayerNumber(int num)
    {
        playerNumber = num;
    }

    public void SetTeam(ETeam teamValue)
    {
        team = teamValue;
    }

    public bool GetIsShrunken()
    {
        return isShrunken;
    }

    public void SetIsShrunken(bool value)
    {
        isShrunken = value;
    }

    public void SetJetpackParticle(ParticleSystem system)
    {
        jetpackParticle = system;
    }

    public void SetFreezeTime(float value)
    {
        remainingFreezeTime = value;
    }

    public bool GetFrozen()
    {
        return isFrozen;
    }

    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
        if(!isFrozen)
        {
            powerupParticleController.PlayPowerupEffect(EPowerUp.Freeze, 1, false);
        }
    }

    public void SetBallDetection(bool value)
    {
        detectionColliderObject.SetActive(value);
    }

    public SpriteRenderer GetSprite()
    {
        return sprite;
    }

    internal Shield GetShield()
    {
        return shieldTransform.GetComponent<Shield>();
    }
    #endregion
}
