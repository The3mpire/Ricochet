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
    [Tooltip("How many hits the player can take")]
    [SerializeField]
    private int hits = 2;

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

    private int currentHits = 0;
    
    private EPowerUp currPowerUp = EPowerUp.None;

    private Player player;

    private GameManager gameManagerInstance = null;

    #endregion


    #region MonoBehaviour
    private void Awake()
    {
        player = ReInput.players.GetPlayer(playerNumber - 1);
    }

    private void OnEnable()
    {
        currentHits = hits;
    }

    void Start()
    {
        body.color = PlayerColorData.getColor(playerNumber, team);
    }

    void Update()
    {
        //TODO state based + infinite jump :)
        RotateShield();
    }

    void FixedUpdate()
    {
        // Cache the horizontal input.
        float h = player.GetAxis("MoveHorizontal");
        float v = player.GetAxis("MoveVertical");

        Vector2 targetVelocity = new Vector2(h, v);
        GetComponent<Rigidbody2D>().velocity = targetVelocity * maxSpeed;
        // movement
        //if (h == 0)
        //{
        //    rigid.velocity = new Vector2(0, rigid.velocity.y);
        //}
        //else
        //{
        //    rigid.AddForce(new Vector2(Mathf.Sign(h) * moveForce, 0f), ForceMode2D.Impulse);
        //}

        //Check if over max speed
        //if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
        //{
        //    rigid.velocity = new Vector2(Mathf.Sign(rigid.velocity.x) * maxSpeed, rigid.velocity.y);
        //}
        //if (Mathf.Abs(rigid.velocity.y) > maxSpeed)
        //{
        //    rigid.velocity = new Vector2(rigid.velocity.x, Mathf.Sign(rigid.velocity.y) * maxSpeed);
        //}

        // direction check
        if (h > 0 && !facingRight)
        {
            Flip();
        }
        else if (h < 0 && facingRight)
        {
            Flip();
        }

        //if (jumpPressed)
        //{
        //    StartJump();
        //    jumpPressed = false;
        //}

        //HoldJump();
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
        float shieldHorizontal = player.GetAxis("RightStickHorizontal");
        float shieldVertical = -player.GetAxis("RightStickVertical");

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

    public bool KillPlayer()
    {
        if (currentHits <= 1)
        {
            rigid.velocity = Vector3.zero;
            gameObject.SetActive(false);
            return true;
        }
        else
        {
            currentHits--;
            return false;
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
