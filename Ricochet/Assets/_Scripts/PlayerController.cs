 using UnityEngine;
using System.Collections;
using Rewired;

public class PlayerController : MonoBehaviour
{

    #region Inspector Variables
    [Header("Movement Settings")]
    public float moveForce = 365f;          
    public float maxSpeed = 5f;
    public float jumpForce = 0.05f;
    public float jumpTime = 1f;  
	public float decayRate = 0.01f;
    public float stickJumpDeadZone = 0.1f;
    public bool stickJump = true;
    [Header("Reference Components")]
    public Transform shieldTransform;
    public AudioClip[] jumpClips;    
	public float timer= 0f;

    public SpriteRenderer body;
    public Rigidbody2D rigid;
    public Transform groundCheck;			
    public Animator anim;                   
    [Header("Other Settings")]
    public int playerNumber = 1;
    public int teamNumber = 1;
   
    #endregion

    #region Hidden Variables
    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool jump = false;
    private Player player;
    private bool grounded = false;          

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

		if ((player.GetButtonDown ("Jump") || (stickJump && (player.GetAxis ("MoveVertical") > stickJumpDeadZone))) && grounded) {
			jump = true;
			timer = 0;
			rigid.AddForce (new Vector2 (0, jumpForce ));
			//StartCoroutine(JumpRoutine());
		} else if (((player.GetButtonDown ("Jump") || (stickJump && (player.GetAxis ("MoveVertical") > stickJumpDeadZone)) && timer < jumpTime && jump))) {
			timer += Time.deltaTime;
			rigid.AddForce (new Vector2 (0, jumpForce *0.3f));
		} else {
			jump = false;
		}
        RotateShield();
    }


    void FixedUpdate()
    {
        // Cache the horizontal input.
        float h = Input.GetAxis("Movement" + playerNumber);

        // The Speed animator parameter is set to the absolute value of the horizontal input.
        anim.SetFloat("Speed", Mathf.Abs(h));
        print(h);

        if (h == 0 && rigid.velocity.x != 0)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        else if (h * rigid.velocity.x < maxSpeed)
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
           // rigid.AddForce(new Vector2(0f, jumpForce));

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

    IEnumerator JumpRoutine()
    {
		//Debug.Log ("Called");
		float timer = 0;
		//rigid.AddForce (Vector2.up * jumpForce); 
		//yield return null;

		/*
		float curForce = jumpForce;
		while ((player.GetButton ("Jump") || (stickJump && (player.GetAxis ("MoveVertical") > stickJumpDeadZone))) && timer < jumpTime && curForce > 0) {
			Debug.Log("alskjfdl;aksf");
			rigid.AddForce (Vector2.up * curForce);        
		    curForce -= decayRate * Time.deltaTime;
			timer += Time.deltaTime;

		}*/

       rigid.velocity = Vector2.zero;
		yield return null;
			
       

       /* while ((player.GetButtonDown("Jump") || (stickJump && (player.GetAxis("MoveVertical") > stickJumpDeadZone))) && timer < jumpTime)
        {
			Debug.Log("alskjfdl;aksf");

            /*float proportionCompleted = timer / jumpTime;
            Vector2 jumpVector = Vector2.up + rigid.velocity;
            Vector2 thisFrameJumpVector = Vector2.Lerp(jumpVector, Vector2.zero, proportionCompleted);
            rigid.AddForce(thisFrameJumpVector);
            timer += Time.deltaTime;
			yield return null;

            
        }*/

		jump = false;			


    }
    #endregion

    void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        body.flipX = !facingRight;
    }
}
