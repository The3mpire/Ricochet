using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using Enumerables;

public class PlayerController : MonoBehaviour
{

    #region Inspector Variables
    [Header("Movement Settings")]
    [Tooltip("How fast the player moves (force)")]
    [SerializeField]
    private float moveForce = 15f;
    [Tooltip("The fastest the player is allowed to move (velocity)")]
    [SerializeField]
    private float maxSpeed = 10f;
    [Tooltip("How strong the player's first jump is (from the ground)")]
    [SerializeField]
    private float initialJumpForce = 8f;
    [Tooltip("How much higher the player goes continues from holding down the button (force)")]
    [SerializeField]
    private float continualJumpForce = 0.9f;
    [Tooltip("How long the player can hold the button before it doesn't do anything")]
    [SerializeField]
    private float maxJumpTime = 0.25f;
    [Tooltip("How strong the player's extra jumps are (already in the air)")]
    [SerializeField]
    private float extraJumpForce = 4f;
    [Tooltip("How sensitive the left stick is before acknowledging input")]
    [SerializeField]
    private float stickJumpDeadZone = 0.01f;
    [Tooltip("Whether the player can jump using the left stick")]
    [SerializeField]
    private bool stickJump = true;
    [Tooltip("How many jumps the player gets while in the air")]
    [SerializeField]
    private int numberOfExtraJumps = 1;

    [Header("Reference Components")]
    [Tooltip("Drag the player's shieldContainer here")]
    [SerializeField]
    private Transform shieldTransform;
    [Tooltip("Drag the player's shield here")]
    [SerializeField]
    private SpriteRenderer shield;
    [Tooltip("Drag the player's body here")]
    [SerializeField]
    private SpriteRenderer body;
    [Tooltip("Drag the player's body here")]
    [SerializeField]
    private Rigidbody2D rigid;
    [Tooltip("Drag the player's \"groundCheck\" here")]
    [SerializeField]
    private Transform groundCheck;

    [Header("Secondary Items")]
    [Tooltip("Drag the player's power up circle shield here")]
    [SerializeField]
    private GameObject circleShield;

    [Header("Other Settings")]
    [Tooltip("Which player this is")]
    [SerializeField]
    private int playerNumber = 1;
    [Tooltip("Which team the player is on")]
    [SerializeField]
    private ETeam team;

    #endregion

    #region Hidden Variables
    private bool facingRight = true;
    private bool jumpPressed = false;
    private bool hasPowerUp = false;
    private bool isJumping = false;
    private bool grounded = false;
    private bool jumpButtonHeld = false;

    private int jumpCounter = 0;
    private float jumpTimer = 0f;   
    
    private EPowerUp currPowerUp = EPowerUp.None;

    private Player player;

    private GameManager gameManagerInstance = null;

    #endregion


    #region MonoBehaviour
    private void Awake()
    {
        player = ReInput.players.GetPlayer(playerNumber - 1);
    }

    void Start()
    {
        body.color = PlayerColorData.getColor(playerNumber, team);
    }

    void Update()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        // frame check
        if (player.GetButton("Jump") || (stickJump && (player.GetAxis("MoveVertical") > stickJumpDeadZone)))
        {
            jumpButtonHeld = true;
        }
        else
        {
            jumpButtonHeld = false;
        }

        // get jump input
        if (player.GetButtonDown("Jump") || (stickJump && (player.GetAxis("MoveVertical") > stickJumpDeadZone)))
        {
            jumpPressed = true;
            jumpButtonHeld = true;
        }
        // player hit the ground
        else if (grounded)
        {
            jumpCounter = 0;
        }

        // player is not pressing button or out of jumps
        if (isJumping && !jumpButtonHeld)
        {
            isJumping = false;
        }
        //TODO state based + infinite jump :)
        RotateShield();
    }

    void FixedUpdate()
    {
        // Cache the horizontal input.
        float h = Input.GetAxis("Movement" + playerNumber);

        // movement
        if (h == 0)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        else
        {
            rigid.AddForce(new Vector2(Mathf.Sign(h) * moveForce, 0f), ForceMode2D.Impulse);
        }

        //Check if over max speed
        if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
        {
            rigid.velocity = new Vector2(Mathf.Sign(rigid.velocity.x) * maxSpeed, rigid.velocity.y);
        }

        // direction check
        if (h > 0 && !facingRight)
        {
            Flip();
        }
        else if (h < 0 && facingRight)
        {
            Flip();
        }

        if (jumpPressed)
        {
            StartJump();
            jumpPressed = false;
        }

        HoldJump();
    }


    #endregion

    #region Private Helpers
    void StartJump()
    {
        //first jump
        if (grounded)
        {
            rigid.AddForce(new Vector2(0, initialJumpForce), ForceMode2D.Impulse);
            jumpCounter++;
            isJumping = true;
            jumpTimer = 0f;
        }
        // extra jumps
        else if (jumpCounter < numberOfExtraJumps)
        {
            if (rigid.velocity.y < 0)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0);
            }
            rigid.AddForce(new Vector2(0, extraJumpForce), ForceMode2D.Impulse);
            jumpCounter++;
            isJumping = true;
            jumpTimer = 0f;
        }
    }

    private void HoldJump()
    {
        if (jumpButtonHeld && isJumping)
        {
            jumpTimer += Time.deltaTime;

            //check if they can still add force
            if (jumpTimer < maxJumpTime)
            {
                rigid.AddForce(new Vector2(0, continualJumpForce), ForceMode2D.Impulse);
            }
        }
    }

    private void RotateShield()
    {
        float shieldHorizontal = Input.GetAxis("ShieldX" + playerNumber);
        float shieldVertical = Input.GetAxis("ShieldY" + playerNumber);

        //make sure there is magnitude
        if (Mathf.Abs(shieldHorizontal) > 0 || Mathf.Abs(shieldVertical) > 0)
        {

            if (shieldHorizontal > 0)
            {
                shieldTransform.localRotation = Quaternion.Euler(new Vector3(shieldTransform.localRotation.eulerAngles.x, shieldTransform.localRotation.eulerAngles.y, -Vector2.Angle(new Vector2(shieldHorizontal, shieldVertical), Vector2.down) + 90));
            }
            else
            {
                shieldTransform.localRotation = Quaternion.Euler(new Vector3(shieldTransform.localRotation.eulerAngles.x, shieldTransform.localRotation.eulerAngles.y, Vector2.Angle(new Vector2(shieldHorizontal, shieldVertical), Vector2.down) + 90));
            }
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        body.flipX = !facingRight;
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

    public void KillPlayer()
    {
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
