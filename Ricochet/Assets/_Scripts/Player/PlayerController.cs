using Enumerables;
using Rewired;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Which player this is")]
    [SerializeField] private int playerNumber = 1;
    [Tooltip("Which team the player is on")]
    [SerializeField] private ETeam team;

    [Header("Movement Settings")]
    [Tooltip("How fast the player moves")]
    [SerializeField] private float moveSpeed = 10f;
    [Tooltip("How fast the player moves during dash")]
    [SerializeField] private float dashSpeed = 35f;
    [Tooltip("How long the dash lasts")]
    [SerializeField] private float dashTime = .1f;
    [Tooltip("How much fuel in seconds to spend on dash")]
    [SerializeField] private float dashCost = 2f;
    [Tooltip("Gravity scale on player")]
    [SerializeField] private float gravScale = 4f;
    [Tooltip("Time in seconds of jetback fuel")]
    [SerializeField] private float startFuel = 5f;
    [Tooltip("Fuel/second recharge when grounded")]
    [SerializeField] private float groundRechargeRate = 2.5f;
    [Tooltip("Fuel/second recharge when grounded")]
    [SerializeField] private float airRechargeRate = 1f;

    [Header("Reference Components")]
    [Tooltip("Drag the player's shieldContainer here")]
    [SerializeField] private Transform shieldTransform;
    [Tooltip("Drag the player's shield here")]
    [SerializeField] private SpriteRenderer shield;
    [Tooltip("Drag the player's bodySprite here")]
    [SerializeField] private SpriteRenderer body;
    [Tooltip("Drag the player's rigidbody here")]
    [SerializeField] private Rigidbody2D rigid;
    [Tooltip("Drag the player's \"groundCheck\" here")]
    [SerializeField] private Transform groundCheck;

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
        rightStickHorz = player.GetAxis("RightStickHorizontal");
        rightStickVert = player.GetAxis("RightStickVertical");
        timeSinceDash += Time.deltaTime;
        
        // Check if still dashing
        if (timeSinceDash >= dashTime)
        {
            dashing = false;
        }

        if (playerNumber == 1 && grounded)
            Debug.Log("help");
        
        BasicMovement();
        DashCheck();
        RotateShield();
    }

    private void FixedUpdate()
    {
        // Find moveDirection
        Vector3 moveDirection;
        if (jumpButtonHeld)
        { 
            moveDirection = new Vector2(leftStickHorz, leftStickVert);
        }
        else
        {
            moveDirection = new Vector2(leftStickHorz, 0);
        }

        // Apply movement speed
        rigid.velocity = moveDirection * moveSpeed;

        // Add dash velocity to movement
        if (dashing)
        {
            rigid.velocity += dashDirection * dashSpeed;
        }

    }
#endregion

#region Helpers
    private void BasicMovement()
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
            dashDirection = new Vector2(rightStickHorz, rightStickVert);
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
        Debug.Log(name + " killed player " + otherPlayer.name);
        killList.Add(otherPlayer);
        //string s = name + " has killed: ";
        //foreach (PC2 p in killList)
        //{
        //    s += p.name + " ";
        //}
        //Debug.Log(s);
    }

    public void PlayerDead()
    {
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

#endregion
}
