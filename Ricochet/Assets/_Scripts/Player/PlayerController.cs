﻿using Enumerables;
using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


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
    [Header("Movement Settings")]
    [Tooltip("Gravity scale on player")]
    [SerializeField]
    private float gravScale = 8f;
    [Tooltip("Acceleration constant while moving laterally")]
    [SerializeField]
    private float lateralAcceleration;
    [Tooltip("Max lateral speed when falling")]
    [SerializeField]
    private float fallingLateralSpeed = 1f;
    [Tooltip("Acceleration/deceleartion constant while using jetpack")]
    [SerializeField]
    private float thrusterAcceleration = 0.05f;
    [Tooltip("Max speed while using jetpack w/ no directional input")]
    [SerializeField]
    private float upThrusterSpeed = 5f;
    [Tooltip("Move speed while using jetpack w/ directional input")]
    [SerializeField]
    private float thrusterSpeed = 15f;
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
    private int motorIndex = 0;
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
    private ParticleSystem powerupParticle;
    [Tooltip("Drag the Tag canvas's tag object here")]
    [SerializeField]
    private Text playerNumberTag;
    [Tooltip("Drag the player's rigidbody here")]
    [SerializeField]
    private Rigidbody2D rigid;
    [Tooltip("Drag the player's \"groundCheck\" here")]
    [SerializeField]
    private Transform groundCheck;
    [Tooltip("Drag the player's \"BurstCollider\" here")]
    [SerializeField]
    private GameObject burstCollider;
    [Tooltip("Drag the player's audio source here")]
    [SerializeField]
    private AudioSource audioSource;
    [Tooltip("Freeze Color")]
    [SerializeField]
    private Color freezeColor;


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
    private bool isFlipped;
    private ParticleSystem jetpackParticle;

    private ECharacter chosenCharacter;
    private EPowerUp currPowerUp = EPowerUp.None;
    private Player player;
    private List<PlayerController> killList;

    private bool infiniteFuel;
    private bool hasPowerUp;
    private bool jetpackBurnedOut;
    private bool isInvincible;
    private bool isFrozen;
    private float remainingFreezeTime;
    
    private float powerUpTimer;
    private Vector2 previousVelocity;

    private float leftStickHorz;
    private float leftStickVert;
    private float rightStickHorz;
    private float rightStickVert;
    private float leftTriggerAxis;

    private bool movementDisabled = false;
    #endregion

    #region Monobehaviour
    private void Awake()
    {
        if (playerNumberTag)
        {
            playerNumberTag.text = playerNumber.ToString();
        }

        powerupParticle.Stop();
        jetpackBurnedOut = false;
        isFrozen = false;

        killList = new List<PlayerController>();
        rigid.gravityScale = 0;
        rightStickHorz = 1;
        rightStickVert = 0;
        shield = GetComponentInChildren<Shield>();
        animator = transform.GetComponentInChildren<Animator>();
        this.isFlipped = this.sprite.flipX;
        team = gameData.GetPlayerTeam(playerNumber - 1);
        chosenCharacter = gameData.GetPlayerCharacter(playerNumber - 1);
        shield.SetTeamColor(team);

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
    }

    private void Update()
    {
        // Update vars
        Vector2 playerPosition = transform.position;
        Vector2 groundChecker = new Vector2(groundCheck.position.x, groundCheck.position.y - 0.1f);

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

        if (!movementDisabled && !isFrozen)
        {
            RotateShield();
            Flip();
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
                    gameManagerInstance.BallPlayerCollision(this.gameObject, collision);
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
                            isFrozen = true;
                            rigid.gravityScale = 0.0f;
                            rigid.velocity = new Vector3(0, 0, 0);
                            sprite.color = freezeColor;
                            otherPlayer.RemovePowerUp();
                            gameManagerInstance.FreezePlayer(this);
                            if(remainingFreezeTime <= 0)
                            {
                                isFrozen = false;
                            }
                        }
                        Rigidbody2D body = collision.gameObject.GetComponent<Rigidbody2D>();
                        body.velocity = otherPlayer.GetPreviousVelocity() * -boingFactor;
                        Rumble(1.25f);
                    }
                }
            }
        }
    }

    #endregion

    #region Helpers

    private void HandleAnimator()
    {
        if (leftTriggerAxis != 0)
        {
            this.animator.SetBool("isJumping", true);
        }
        else if (IsGrounded())
        {
            this.animator.SetBool("isJumping", false);

            if (rigid.velocity != Vector2.zero)
            {
                this.animator.SetBool("isWalking", true);
            }
            else
            {
                this.animator.SetBool("isWalking", false);
            }
        }

        if (rigid.velocity.magnitude > thrusterSpeed)
        {
            this.animator.SetTrigger("dash");
        }
    }

    private void Move()
    {
        Vector2 moveDirection;

        if (leftTriggerAxis != 0)
        {
            moveDirection = new Vector2(leftStickHorz, leftStickVert).normalized;
            
            if (this.jetpackParticle && !this.jetpackParticle.isPlaying && !isFrozen)
            {
                this.jetpackParticle.Play();
            }
            // If there is no directional input, decelerate movement to a still hover
            if (moveDirection == Vector2.zero)
            {
                moveDirection = Vector2.up;
                float x = Mathf.Abs(rigid.velocity.x) > 0.1 ? rigid.velocity.x * 0.8f : 0f;
                float y = Mathf.Abs(rigid.velocity.y) > 0.5f ? rigid.velocity.y * 0.90f : 0;
                rigid.velocity = new Vector2(x, y);
            } // else go in the direction of input with thruster speed
            else
            {
                float x, y;
                x = moveDirection.x > 0 ? Mathf.Min(Mathf.Max(rigid.velocity.x, rigid.velocity.x * directionSwitchRatio) + (moveDirection.x * thrusterAcceleration * leftTriggerAxis), thrusterSpeed) :
                    Mathf.Max(Mathf.Min(rigid.velocity.x, rigid.velocity.x * directionSwitchRatio) + (moveDirection.x * thrusterAcceleration * leftTriggerAxis), -thrusterSpeed);

                y = moveDirection.y >= 0 ? Mathf.Min(Mathf.Max(rigid.velocity.y, rigid.velocity.y * directionSwitchRatio) + (moveDirection.y * thrusterAcceleration * leftTriggerAxis), thrusterSpeed) :
                    Mathf.Max(Mathf.Min(rigid.velocity.y, rigid.velocity.y * directionSwitchRatio) + (moveDirection.y * thrusterAcceleration * leftTriggerAxis), -thrusterSpeed);

                rigid.velocity = new Vector2(x, y);
            }
        }
        else
        { // if jetpack is not engaged, only move horizontally with groundedMoveSpeed or airMovespeed
            moveDirection = new Vector2(leftStickHorz, 0).normalized;
            if (this.jetpackParticle)
            {
                this.jetpackParticle.Stop();
            }
            
            if (IsGrounded())
            {
                rigid.velocity = moveDirection * groundedMoveSpeed;
            }
            else
            {
                float x = 0, y = 0;
                if (rigid.velocity.x > fallingLateralSpeed)
                {
                    x = rigid.velocity.x - (lateralAcceleration * 2f);
                }
                else if (rigid.velocity.x < -fallingLateralSpeed)
                {
                    x = rigid.velocity.x + (lateralAcceleration * 2f);
                }
                else
                {
                    if (moveDirection.x > 0)
                    {
                        x = Mathf.Min(rigid.velocity.x + (moveDirection.x * lateralAcceleration * 0.5f), fallingLateralSpeed);
                    }
                    else if (moveDirection.x < 0)
                    {
                        x = Mathf.Max(rigid.velocity.x + (moveDirection.x * lateralAcceleration * 0.5f), -fallingLateralSpeed);
                    }
                }
                y = Mathf.Max(rigid.velocity.y - 0.5f, -airMoveSpeed);
                rigid.velocity = new Vector2(x, y);
            }
        }
    }

    private void Flip()
    {
        if (leftStickHorz < 0)
        {
            sprite.flipX = isFlipped;
        }
        else if (leftStickHorz > 0)
        {
            sprite.flipX = !isFlipped;
        }
    }

    private void RotateShield()
    {
        //make sure there is magnitude
        if (Mathf.Abs(rightStickHorz) > 0 || Mathf.Abs(rightStickVert) > 0)
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
    #endregion

    #region External Functions
    public void Push(Vector3 direction, float force)
    {
        Debug.Log("pushed");

        Vector3 result = direction * force;
        rigid.AddForce(result);
    }
    public void Rumble(float multiplier = 1f)
    {
        player.SetVibration(motorIndex, motorLevel * multiplier, rumbleDuration * multiplier);
    }

    public void ReceivePowerUp(EPowerUp powerUp, Color powerUpColor)
    {
        ParticleSystem.MainModule sparks = powerupParticle.main;
        ParticleSystem.MainModule orb = powerupParticle.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        sparks.startColor = powerUpColor;
        orb.startColor = powerUpColor;
        if (powerupParticle.isStopped)
        {
            powerupParticle.Play();
        }
        hasPowerUp = true;
        currPowerUp = powerUp;
    }

    public void AddVelocity(Vector2 velocity)
    {
        rigid.velocity += velocity;
    }

    public void RemovePowerUp()
    {
        if (powerupParticle.isPlaying)
        {
            powerupParticle.Stop();
        }
        hasPowerUp = false;
        EnableSecondaryShield(false);
        currPowerUp = EPowerUp.None;
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
        StartCoroutine(gameData.Blink(gameData.playerRespawnTime,sprite));
        StartCoroutine(KillPlayer());
    }

    private IEnumerator KillPlayer()
    {
        this.animator.SetBool("isDead", true);
        isInvincible = true;
        movementDisabled = true;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        yield return new WaitForSeconds(gameData.playerRespawnTime);
        this.animator.SetBool("isDead", false);
        isInvincible = false;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
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
    public bool IsGrounded()
    {
        Vector2 groundChecker = new Vector2(groundCheck.position.x, groundCheck.position.y - 0.1f);

        bool touchingGround = Physics2D.Linecast(transform.position, groundChecker, 1 << LayerMask.NameToLayer("Ground"));
        bool jetpackActive = leftTriggerAxis != 0;

        return touchingGround && !jetpackActive;
    }

    public Vector2 GetRightStickDirection()
    {
        return new Vector2(rightStickHorz, rightStickVert).normalized;
    }

    public Player GetPlayer()
    {
        return ReInput.players.GetPlayer(playerNumber - 1);
    }

    public ECharacter GetCharacter()
    {
        return chosenCharacter;
    }

    public void DisableMovement(bool movementDisabled)
    {
        this.movementDisabled = movementDisabled;
    }

    public void SetInfiniteFuel(bool active)
    {
        infiniteFuel = active;
    }

    public Transform GetShieldTransform()
    {
        return shieldTransform;
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

    public void SetJetpackParticle(ParticleSystem system)
    {
        this.jetpackParticle = system;
    }

    public void SetFreezeTime(float value)
    {
        remainingFreezeTime = value;
    }

    internal Shield GetShield()
    {
        return shieldTransform.GetComponent<Shield>();
    }

    private Sprite GetCharacterSprite(Enumerables.ECharacter character)
    {
        Sprite charSprite = null;

        switch (character)
        {
            case ECharacter.CatManWT:
                charSprite = Resources.Load<Sprite>("_Art/2D Sprites/Characters/catWalkPreview");
                break;
            case ECharacter.CatManP:
                charSprite = Resources.Load<Sprite>("_Art/2D Sprites/Characters/catWalkPreviewAlt");
                break;
            case ECharacter.Computer:
                charSprite = Resources.Load<Sprite>("_Art/2D Sprites/Characters/Y2K_01");
                break;
            case ECharacter.MallCop:
                charSprite = Resources.Load<Sprite>("_Art/2D Sprites/Characters/Forward");
                break;
        }
        return charSprite;
    }
    #endregion
}
