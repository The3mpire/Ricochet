 using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    #region Inspector Variables
    [Header("Movement Settings")]
    public float moveForce = 365f;          // Amount of force added to move the player left and right.
    public float maxSpeed = 5f;             // The fastest the player can travel in the x axis.
    public float jumpForce = 1000f;         // Amount of force added when the player jumps.
    [Header("Reference Components")]
    public Transform shieldTransform;
    public AudioClip[] jumpClips;           // Array of clips for when the player jumps.

    public SpriteRenderer body;
    public Rigidbody2D rigid;
    public Transform groundCheck;			// A position marking where to check if the player is grounded.
    public Animator anim;                   // Reference to the player's animator component.
    [Header("Other Settings")]
    public int playerNumber = 1;
    public int teamNumber = 1;
    public Color team1Color = Color.white;
    public Color team2Color = Color.red;
    #endregion

    #region Hidden Variables
    [HideInInspector]
    public bool facingRight = true;         // For determining which way the player is currently facing.
    [HideInInspector]
    public bool jump = false;               // Condition for whether the player should jump

    private bool grounded = false;          // Whether or not the player is grounded.

    #endregion


    #region MonoBehaviour
    void Start()
    {
        switch (teamNumber)
        {
            case 1:
                body.color = team1Color;
                break;
            case 2:
                body.color = team2Color;
                break;
        }
    }

    void Update()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButtonDown("Jump" + playerNumber) && grounded)
            jump = true;

        RotateShield();
    }


    void FixedUpdate()
    {
        // Cache the horizontal input.
        float h = Input.GetAxis("Movement" + playerNumber);

        // The Speed animator parameter is set to the absolute value of the horizontal input.
        anim.SetFloat("Speed", Mathf.Abs(h));

        if (h * rigid.velocity.x < maxSpeed)
            rigid.AddForce(Vector2.right * h * moveForce);
        else if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
            rigid.velocity = new Vector2(Mathf.Sign(rigid.velocity.x) * maxSpeed, rigid.velocity.y);

        if (h > 0 && !facingRight)
        {
            Flip();
        }
        else if (h < 0 && facingRight)
        {
            Flip();
        }

        if (jump)
        {
            // Set the Jump animator trigger parameter.
            anim.SetTrigger("Jump");

            // Play a random jump audio clip.
            int i = Random.Range(0, jumpClips.Length);
            AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

            // Add a vertical force to the player.
            rigid.AddForce(new Vector2(0f, jumpForce));

            // Make sure the player can't jump again until the jump conditions from Update are satisfied.
            jump = false;
        }
    }

    void RotateShield()
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
    #endregion


    void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        body.flipX = !facingRight;
    }
}
