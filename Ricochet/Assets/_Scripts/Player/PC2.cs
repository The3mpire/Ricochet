using Enumerables;
using Rewired;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PC2 : MonoBehaviour
{
#region Inspector Variables
    [Header("Movement Settings")]
    [Tooltip("How fast the player moves")]
    [SerializeField] private float moveSpeed = 7f;
    [Tooltip("Gravity Scale on player")]
    [SerializeField] private float gravScale = 5f;

    [Tooltip("How strong the player's first jump is (from the ground)")]
    [SerializeField] private float initialJumpForce = 8f;
    [Tooltip("How much higher the player goes continues from holding down the button (force)")]
    [SerializeField] private float continualJumpForce = 0.9f;
    [Tooltip("How long the player can hold the button before it doesn't do anything")]
    [SerializeField] private float maxJumpTime = 0.25f;
    [Tooltip("How strong the player's extra jumps are (already in the air)")]
    [SerializeField] private float extraJumpForce = 4f;
    [Tooltip("How sensitive the left stick is before acknowledging input")]
    [SerializeField] private float stickJumpDeadZone = 0.01f;
    [Tooltip("Whether the player can jump using the left stick")]
    [SerializeField] private bool stickJump = true;
    [Tooltip("How many jumps the player gets while in the air")]
    [SerializeField] private int numberOfExtraJumps = 1;

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

    [Header("Other Settings")]
    [Tooltip("Which player this is")]
    [SerializeField] private int playerNumber = 1;
    [Tooltip("Which team the player is on")]
    [SerializeField] private ETeam team;
#endregion

#region Hidden Variables
    private bool grounded;
    private EPowerUp currPowerUp = EPowerUp.None;
    private Player player;
    private List<PC2> killList;
    private GameManager gameManagerInstance;
    private bool jumpButtonHeld;
    private float leftStickHorz;
    private float leftStickVert;
    private float rightStickHorz;
    private float rightStickVert;
#endregion

#region Monobehaviour
    private void Awake()
    {
        killList = new List<PC2>();
    }

    private void Start()
    {
        player = ReInput.players.GetPlayer(playerNumber - 1);
        body.color = PlayerColorData.getColor(playerNumber, team);
    }
	
	private void Update()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        jumpButtonHeld = false;
        rigid.gravityScale = gravScale;
        leftStickHorz = player.GetAxis("MoveHorizontal");
        leftStickVert = player.GetAxis("MoveVertical");
        rightStickHorz = player.GetAxis("RightStickHorizontal");
        rightStickVert = -player.GetAxis("RightStickVertical");

        if (player.GetButton("Jump"))
        {
            rigid.gravityScale = 0;
            jumpButtonHeld = true;
            grounded = false;
        }

        RotateShield();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection;
        if (jumpButtonHeld)
        { 
            moveDirection = new Vector3(leftStickHorz, leftStickVert, 0);
        }
        else
        {
            moveDirection = new Vector3(leftStickHorz, 0, 0);
        }

        rigid.velocity = moveDirection * moveSpeed;
    }
#endregion

    private void RotateShield()
    {
        //make sure there is magnitude
        if (Mathf.Abs(rightStickHorz) > 0 || Mathf.Abs(rightStickVert) > 0)
        {

            if (rightStickHorz > 0)
            {
                shieldTransform.localRotation = Quaternion.Euler(new Vector3(shieldTransform.localRotation.eulerAngles.x, shieldTransform.localRotation.eulerAngles.y, -Vector2.Angle(new Vector2(rightStickHorz, rightStickVert), Vector2.down) + 90));
            }
            else
            {
                shieldTransform.localRotation = Quaternion.Euler(new Vector3(shieldTransform.localRotation.eulerAngles.x, shieldTransform.localRotation.eulerAngles.y, Vector2.Angle(new Vector2(rightStickHorz, rightStickVert), Vector2.down) + 90));
            }
        }
    }

}
