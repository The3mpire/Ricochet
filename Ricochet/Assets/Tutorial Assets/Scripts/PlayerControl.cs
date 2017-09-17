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
    public AudioClip[] jumpClips;           // Array of clips for when the player jumps.

    public SpriteRenderer body;
    public Rigidbody2D rigid;
    public Transform groundCheck;			// A position marking where to check if the player is grounded.
    public Animator anim;                   // Reference to the player's animator component.
    [Header("Other Settings")]
    public int playerNumber = 1;
    #endregion

    #region Hidden Variables
    [HideInInspector]
    public bool facingRight = true;         // For determining which way the player is currently facing.
    [HideInInspector]
    public bool jump = false;               // Condition for whether the player should jump

    private bool grounded = false;          // Whether or not the player is grounded.

    #endregion


    #region MonoBehaviour
    void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  

		// If the jump button is pressed and the player is grounded then the player should jump.
		if(Input.GetButtonDown("Jump" + playerNumber) && grounded)
			jump = true;
	}


	void FixedUpdate ()
	{
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal" + playerNumber);

		// The Speed animator parameter is set to the absolute value of the horizontal input.
		anim.SetFloat("Speed", Mathf.Abs(h));

        //player speed
		if(h * rigid.velocity.x < maxSpeed)
            rigid.AddForce(Vector2.right * h * moveForce);
		else if(Mathf.Abs(rigid.velocity.x) > maxSpeed)
            rigid.velocity = new Vector2(Mathf.Sign(rigid.velocity.x) * maxSpeed, rigid.velocity.y);

		// set sprite correctly
		if(h > 0 && !facingRight)
			Flip();
		else if(h < 0 && facingRight)
			Flip();
        
		if(jump)
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
    #endregion


    void Flip ()
	{
		facingRight = !facingRight;
        body.flipX = !facingRight;
	}
}
