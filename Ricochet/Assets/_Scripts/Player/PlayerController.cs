﻿using Enumerables;
using Rewired;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    #region Instance Variables
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
    [Tooltip("Move speed while using jetpack w/ no directional input")]
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
    [Tooltip("How quickly the player can accelerate")]
    [SerializeField] private float inertia = 1.5f;

    [Header("Fuel Settings")]
    [Tooltip("Time in seconds of jetback fuel")]
    [SerializeField]
    private float startFuel = 7f;
    [Tooltip("Fuel/second recharge when grounded")]
    [SerializeField]
    private float groundRechargeRate = 3.5f;
    [Tooltip("Fuel/second recharge when falling")]
    [SerializeField]
    private float airRechargeRate = 1.5f;

    [Header("Dash Settings")]
    [Tooltip("How fast the player moves during dash")]
    [SerializeField]
    private float dashSpeed = 35f;
    [Tooltip("How long the dash lasts")]
    [SerializeField]
    private float dashTime = .2f;
    [Tooltip("How much fuel in seconds to spend on dash")]
    [SerializeField]
    private float dashCost = 2f;

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
    [Tooltip("Drag the player's rigidbody here")]
    [SerializeField]
    private Rigidbody2D rigid;
    [Tooltip("Drag the player's \"groundCheck\" here")]
    [SerializeField]
    private Transform groundCheck;

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


    private EPowerUp currPowerUp = EPowerUp.None;
    private Player player;
    private List<PlayerController> killList;

    private bool infiniteFuel;
    private bool hasPowerUp;
    private bool dashing;
    private bool grounded;
    private bool jumpButtonHeld;

    private float currentFuel;
    private float maxFuel;
    private float powerUpTimer;
    private float timeSinceDash;
    private Vector2 dashDirection;
    private int inertiaTime;
    private bool inertiaSwitch;

    private float leftStickHorz;
    private float leftStickVert;
    private float rightStickHorz;
    private float rightStickVert;
    private float leftTriggerAxis;
    #endregion

    #region Monobehaviour
    private void Awake()
    {
		if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
		{
			gameManagerInstance.LoadPlayer(this, playerNumber);
		}

        powerupParticle.Stop();
			
        killList = new List<PlayerController>();
        currentFuel = startFuel;
        maxFuel = startFuel;
        timeSinceDash = 0f;
        inertiaTime = 2;
        rightStickHorz = 1;
        rightStickVert = 0; 
        shield = GetComponentInChildren<Shield>();
        animator = transform.GetComponentInChildren<Animator>();
        this.isFlipped = this.sprite.flipX;
        team = GameData.playerTeams == null ? ETeam.BlueTeam : GameData.playerTeams[playerNumber - 1];
        shield.SetTeamColor(team);
        gameManagerInstance.NoWaitRespawnAllPlayers();
        //SetBodyType(GetCharacterSprite(GameData.playerCharacters[playerNumber-1]));
    }

    private void Start()
    {
        player = ReInput.players.GetPlayer(playerNumber - 1);
        sprite.color = PlayerColorData.getColor(playerNumber, team);
        ECharacter chosenCharacter = GameData.playerCharacters != null ? GameData.playerCharacters[playerNumber - 1] : ECharacter.MallCop;
        transform.GetComponentInChildren<BasePlayerSetup>().SetupCharacter(chosenCharacter, 0);
    }

    private void Update()
    {
        // Update vars
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        jumpButtonHeld = false;
        leftStickHorz = player.GetAxis("MoveHorizontal");
        leftStickVert = player.GetAxis("MoveVertical");
        if (player.GetAxis("RightStickHorizontal") != 0)
        {
            rightStickHorz = player.GetAxis("RightStickHorizontal");
        }
        if (player.GetAxis("RightStickVertical") != 0)
        {
            rightStickVert = player.GetAxis("RightStickVertical");
        }
        timeSinceDash += Time.deltaTime;

        // Check if still dashing
        if (timeSinceDash >= dashTime)
        {
            dashing = false;
        }

        MovementPreperation();
        DashCheck();
        RotateShield();
        Flip();
    }

    private void FixedUpdate()
    {
        Move();

        // Add dash velocity to movement
        if (dashing)
        {
            rigid.velocity += dashDirection * dashSpeed;
        }
    }
    #endregion

    #region Collision Management

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
            {
                Ball ball = collision.gameObject.GetComponent<Ball>();
                gameManagerInstance.BallPlayerCollision(this.gameObject, ball);
                ball.ReverseBall();
            }
        }
    }

    #endregion

    #region Helpers

    private void MovementPreperation()
    {
        if (leftTriggerAxis != 0 && currentFuel > 0)
        {
            grounded = false;
            currentFuel -= Time.deltaTime * leftTriggerAxis;
        } // recharge fuel on ground
        else if (grounded && currentFuel < maxFuel)
        {
            currentFuel += groundRechargeRate * Time.deltaTime;
        } // recharge fuel in air
        else if (currentFuel < maxFuel)
        {
            currentFuel += airRechargeRate * Time.deltaTime;
        }
        leftTriggerAxis = player.GetAxis("Jetpack");
        InertiaFunction("logarithmic", leftTriggerAxis != 0);
    }

    private void Move()
    {
        Vector2 moveDirection;
        float fuelFactor = (currentFuel > 0) ? 1f : 0.05f;

        if (leftTriggerAxis != 0)
        {
            moveDirection = new Vector2(leftStickHorz, leftStickVert).normalized;
            moveDirection *= fuelFactor;
            this.animator.SetBool("isJumping", true);
            // If there is no directional input, just go up with upThrusterSpeed
            if (moveDirection == Vector2.zero)
            {
                moveDirection = Vector2.up * fuelFactor;
                rigid.velocity = moveDirection * upThrusterSpeed * leftTriggerAxis;
            } // else go in the direction of input with thruster speed
            else
            {
                rigid.velocity = moveDirection * thrusterSpeed * leftTriggerAxis;
            }
        }
        else
        { // if jetpack is not engaged, only move horizontally with groundedMoveSpeed or airMovespeed
            moveDirection = new Vector2(leftStickHorz, 0).normalized;
            if(moveDirection != Vector2.zero)
            {
                this.animator.SetBool("isWalking", true);
            } else
            {
                this.animator.SetBool("isWalking", false);
            }
            if (grounded)
            {
                this.animator.SetBool("isJumping", false);
                rigid.velocity = moveDirection * groundedMoveSpeed;
            }
            else
            {
                rigid.velocity = moveDirection * airMoveSpeed;
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


    private void DashCheck()
    {
        if (player.GetButtonDown("Dash") && !dashing && (currentFuel >= dashCost || infiniteFuel))
        {
            this.animator.SetTrigger("dash");
            currentFuel -= dashCost;
            dashDirection = new Vector2(rightStickHorz, rightStickVert).normalized;
            dashing = true;
            timeSinceDash = 0f;
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

    private void InertiaFunction(string function, bool acc)
    {
        if (!grounded)
        {
            switch (function)
            {
                case "linear":
                    if (acc)
                    {
                        rigid.gravityScale += (rigid.gravityScale > 0) ? -inertia : 0 - rigid.gravityScale;
                    }
                    else
                    {
                        rigid.gravityScale += (rigid.gravityScale < gravScale) ? inertia : gravScale - rigid.gravityScale;
                    }
                    break;
                case "logarithmic":
                    inertiaTime = (acc == inertiaSwitch) ? (inertiaTime + 1) : 1;
                    inertiaSwitch = acc;
                    if (acc)
                    {
                        rigid.gravityScale = (rigid.gravityScale <= 0) ? 0 : rigid.gravityScale - Mathf.Log((inertiaTime / inertia) + 1);
                    }
                    else
                    {
                        rigid.gravityScale = (rigid.gravityScale >= gravScale) ? gravScale : rigid.gravityScale + Mathf.Log((inertiaTime / inertia) + 1);
                    }
                    break;
                case "none":
                    if (acc)
                    {
                        rigid.gravityScale = 0;
                    }
                    else
                    {
                        rigid.gravityScale = 8;
                    }
                    break;
            }
        }

    }
    #endregion

    #region External Functions
    public void ReceivePowerUp(EPowerUp powerUp, Color shieldColor)
    {
        if (powerupParticle.isStopped)
        {
            powerupParticle.Play();
        }
        hasPowerUp = true;
        currPowerUp = powerUp;
        //TODO set particle effect color
        //Might want this latter when we have different sprites to represent team colors
        //shield.SetColor(shieldColor);
    }

	public void RemovePowerUp(Color sheildColor)
    {
        if (powerupParticle.isPlaying)
        {
            powerupParticle.Stop();
        }
        hasPowerUp = false;
        EnableSecondaryShield(false);
        currPowerUp = EPowerUp.None;
        //Add this back in at a later point
		//shield.SetColor (sheildColor);
    }

    public void RegisterKill(PlayerController otherPlayer)
    {
        killList.Add(otherPlayer);
    }

    public void PlayerDead()
    {
        currentFuel = startFuel;
        maxFuel = startFuel;
        dashing = false;
        timeSinceDash = 0f;
        rigid.velocity = Vector3.zero;
        gameObject.SetActive(false);
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
    public void SetInfiniteFuel(bool active)
    {
        infiniteFuel = active;
    }

    public Transform GetShieldTransform()
    {
        return shieldTransform;
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

    public void SetPlayerNumber(int num)
    {
        playerNumber = num;
    }

    public void SetTeam(ETeam teamValue)
    {
        team = teamValue;
    }

    internal float GetMaxFuel()
    {
        return maxFuel;
    }

    internal float GetCurrentFuel()
    {
        return currentFuel;
    }

    internal Shield GetShield()
    {
        return shieldTransform.GetComponent<Shield>();
    }

    private Sprite GetCharacterSprite(Enumerables.ECharacter character)
    {
		Debug.Log (character);
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
				charSprite = Resources.Load<Sprite> ("_Art/2D Sprites/Characters/Y2K_01");
                break;
            case ECharacter.MallCop:
                charSprite = Resources.Load<Sprite>("_Art/2D Sprites/Characters/Forward");
                break;
        }
        return charSprite;
    }
    #endregion
}