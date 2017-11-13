using Enumerables;
using Rewired;
using System.Collections.Generic;
using UnityEngine;

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
    [Tooltip("Drag the player's shieldContainer here")]
    [SerializeField]
    private Transform shieldTransform;
    [Tooltip("Drag the player's shield here")]
    [SerializeField]
    private SpriteRenderer shield;
    [Tooltip("Drag the player's bodySprite here")]
    [SerializeField]
    private SpriteRenderer body;
    [Tooltip("Drag the player's rigidbody here")]
    [SerializeField]
    private Rigidbody2D rigid;
    [Tooltip("Drag the player's \"groundCheck\" here")]
    [SerializeField]
    private Transform groundCheck;
#endregion

#region Hidden Variables
    private GameManager gameManagerInstance;
    
    private EPowerUp currPowerUp = EPowerUp.None;
    private Player player;
    private List<PlayerController> killList;
    
    private bool hasPowerUp;
    private bool dashing;
    private bool grounded;
    private bool jumpButtonHeld;

    private float currentFuel;
    private float maxFuel;
    private float timeSinceDash;
    private Vector2 dashDirection;

    private float leftStickHorz;
    private float leftStickVert;
    private float rightStickHorz;
    private float rightStickVert;
#endregion

#region Monobehaviour
    private void Awake()
    {
        killList = new List<PlayerController>();
        currentFuel = startFuel;
        maxFuel = startFuel;
        timeSinceDash = 0f;
        rightStickHorz = 1;
        rightStickVert = 0;
    }

    private void Start()
    {
        player = ReInput.players.GetPlayer(playerNumber - 1);
        body.color = PlayerColorData.getColor(playerNumber, team);
    }
	
	private void Update()
    {
        // Update vars
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        jumpButtonHeld = false;
        rigid.gravityScale = gravScale;
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

#region Helpers
    private void Move()
    {
        Vector2 moveDirection;
        if (jumpButtonHeld)
        {
            moveDirection = new Vector2(leftStickHorz, leftStickVert).normalized;

            // If there is no directional input, just go up with upThrusterSpeed
            if (moveDirection == Vector2.zero)
            {
                moveDirection = Vector2.up;
                rigid.velocity = moveDirection * upThrusterSpeed;
            } // else go in the direction of input with thruster speed
            else
            {
                rigid.velocity = moveDirection * thrusterSpeed;
            }
        }
        else
        { // if jetpack is not engaged, only move horizontally with groundedMoveSpeed or airMovespeed
            moveDirection = new Vector2(leftStickHorz, 0).normalized;
            if (grounded)
            {
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
        if (leftStickHorz < 0 && !body.flipX)
        {
            body.flipX = true;
        }
        else if (leftStickHorz > 0 && body.flipX)
        {
            body.flipX = false;
        }
    }

    private void MovementPreperation()
    {
        if (player.GetButton("Jump") && currentFuel > 0)
        {
            rigid.gravityScale = 0;
            jumpButtonHeld = true;
            grounded = false;
            currentFuel -= Time.deltaTime;
        } // recharge fuel on ground
        else if (grounded && currentFuel < maxFuel)
        {
            currentFuel += groundRechargeRate * Time.deltaTime;
        } // recharge fuel in air
        else if (currentFuel < maxFuel)
        {
            currentFuel += airRechargeRate * Time.deltaTime;
        }
    }

    private void DashCheck()
    {
        if (player.GetButtonDown("Dash") && !dashing && currentFuel >= dashCost)
        {
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
        }
    }
#endregion

#region External Functions
    public void ReceivePowerUp(EPowerUp powerUp)
    {
        hasPowerUp = true;
        currPowerUp = powerUp;
    }

    public void RemovePowerUp()
    {
        hasPowerUp = false;
        currPowerUp = EPowerUp.None;
        shield.color = Color.white;
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
#endregion

#region Getters and Setters
    public SpriteRenderer GetShieldSpriteRenderer()
    {
        return shield;
    }

    public EPowerUp GetCurrentPowerUp()
    {
        return currPowerUp;
    }

    public ETeam GetTeamNumber()
    {
        return team;
    }

    internal float GetMaxFuel()
    {
        return maxFuel;
    }

    internal float GetCurrentFuel()
    {
        return currentFuel;
    }

    #endregion
}
