using UnityEngine;

public class PlayerMovememt : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField]private float jumpForce = 20f;

    [Header("Coyote Time")] //How much time the player can jump after falling off a platform
    [SerializeField] private float coyoteTime;
    private float coyoteCounter; //Count for how much time the player has for coyote time

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps; //How many extra jumps the player can do
    private int jumpCounter;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpX; //Horizontal force for wall jump
    [SerializeField] private float wallJumpY; //Vertical force for wall jump

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("SFX")]
    [SerializeField] private AudioClip jumpSound;
    private Rigidbody2D body;
    private BoxCollider2D box;
    private Animator anim;
    private float horizontal;



    private void Awake()
    {
        //Get references for components
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");

        //Flip character right or left
        if(horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if(horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }



        if(Input.GetKey(KeyCode.LeftShift))
        {
            horizontal *= Dash();
        }
        else
        {
            anim.SetBool("IsDash", false);
        }
        if(Input.GetKey(KeyCode.LeftControl))
        {
            horizontal /= 2;
        }

        anim.SetBool("IsRun", horizontal != 0);
        anim.SetBool("IsGrounded", IsGrounded());

        //Jump logic
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        //Adjustable height jump
        if(Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);
        }

        if(OnWall())
        {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontal * speed, body.velocity.y);

            if(IsGrounded())
            {
                coyoteCounter = coyoteTime; //Reset coyote time when player is grounded
                jumpCounter = extraJumps; //Reset extra jumps when player is grounded
            }
            {
                coyoteCounter -= Time.deltaTime; //Start counting down coyote time
            }
        }
    }

    private void Jump()
    {
        //if coyote time is over and player is not on wall and no extra jump, return
        if(coyoteCounter <= 0 && !OnWall() && jumpCounter <=0) return;

        AudioManager.instance.PlayAudioClip(jumpSound);

        if (OnWall())
        {
            WallJump();
        }
        else
        {
            if (IsGrounded())
            {
                body.velocity = new Vector2(body.velocity.x, jumpForce);
            }
            else
            {
                //if not grounded, check if coyote time is still active
                if (coyoteCounter > 0)
                {
                    body.velocity = new Vector2(body.velocity.x, jumpForce);
                }
                else
                {
                    //if extra jumps are available, use them 
                    if(jumpCounter > 0)
                    {
                        body.velocity = new Vector2(body.velocity.x, jumpForce);
                        jumpCounter--;
                    }
                }
            }
            //Reset the coyote time
            coyoteCounter = 0;
        }
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        
    }

    private float Dash()
    {
        anim.SetBool("IsDash",true);
        return 1.5f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, new Vector2(transform.localScale.x,0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool CanAttack()
    {
        return horizontal == 0 && IsGrounded() && !OnWall();
    }
    
    public bool IsOnWall()
    {
        return OnWall();
    }
 
}
