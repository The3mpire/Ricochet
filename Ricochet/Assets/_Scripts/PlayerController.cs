using UnityEngine;
using System.Collections;
using Rewired;
using Enumerables;

public class PlayerController : MonoBehaviour
{

    #region Inspector Variables
    [Header("Movement Settings")]
    public float moveForce = 365f;
    public float maxSpeed = 5f;
    public float initialJumpForce = 1f;
    public float continualJumpForce = 0.005f;
    public float maxJumpTime = 1f;
    public float extraJumpForce = .5f;
    public float decayRate = 0.01f;
    public float stickJumpDeadZone = 0.1f;
    public bool stickJump = true;
    public int numberOfExtraJumps = 2;

    [Header("Reference Components")]
    public Transform shieldTransform;
    public SpriteRenderer shield;
    public SpriteRenderer body;
    public Rigidbody2D rigid;
    public Transform groundCheck;

    [Header("Other Settings")]
    public int playerNumber = 1;
    public int teamNumber = 1;

    #endregion

    #region Hidden Variables
    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool jumpPressed = false;
    private bool hasPowerUp = false;
    private bool isJumping = false;
    [SerializeField]
    private bool grounded = false;
    [SerializeField]
    private int jumpCounter = 0;
    private float jumpTimer = 0f;
    private bool jumpButtonHeld = false;

    private Player player;


    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        player = ReInput.players.GetPlayer(playerNumber - 1);
    }

    void Start()
    {
        body.color = PlayerColorData.getColor(playerNumber, teamNumber);
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

    #region PrivateHelpers
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

    #region ExternalFunctions

    public void ReceivePowerUp(EPowerUp powerUp)
    {
        switch (powerUp)
        {
            case EPowerUp.InfiniteJump:
                Debug.LogError("InfiniteJump not implemented", gameObject);
                break;
            case EPowerUp.Multiball:
                break;
        }
    }

    #endregion
}
