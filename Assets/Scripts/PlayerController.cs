using UnityEngine;

using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    enum Direction : int
    {
        Left = -1,
        Right = 1,
        None = 0
    }

    //TODO Use
    enum State
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Dashing,
        Spinning
    }

    [SerializeField] private float moveSpeed = 5f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityScale = 3f;
    /// <summary>
    /// How many seconds after leaving a platform the player is still allowed to jump. 
    /// </summary>
    [SerializeField] private float coyoteTime = 0.2f;
    /// <summary>
    /// How many seconds after pressing jump button a jump can occur on landing. 
    /// </summary>
    [SerializeField] private float jumpBufferTime = 0.2f;
    

    [Header("Dashing")]
    [SerializeField] private int numDashes = 1; 
    [SerializeField] private float dashSpeed = 5f;
    /// <summary>
    /// How long the dash lasts.
    /// </summary>
    [SerializeField] private float dashTime = 0.5f;

    [Header("Twirling")]
    [SerializeField] private int numTwirls = 1; 
    [SerializeField] private float twirlSpeed = 15f;
    /// <summary>
    /// How long after a dash is started the player can start a twirl.
    /// </summary>
    [SerializeField] private float canTwirlTime = 3.5f;
    

    [Header("Collision Casts")]
    /// <summary>
    /// How far below the player a platform can be while it's counted as standing on ground. 
    /// </summary>
    [SerializeField] private float groundCheckBoxHeight = 0.1f;
    //[SerializeField] private float groundCheckBoxWidth = 1f;
    private float groundCheckBoxWidth;


    /// <summary>
    /// How far in front of the player a wall can be while it's counted as facing a wall. 
    /// </summary>
    [SerializeField] private float wallCheckDistance = 0.1f; //TODO Use
    

    [Header("Components")]
    // Defines which layers should be stood on
    [SerializeField] private LayerMask groundLayer;
    // Which particles to play on jump or landing
    [SerializeField] private ParticleSystem dustParticleSystem;
    [SerializeField] private ParticleSystem twirlParticleSystem;

    public CollisionCombat cc; // CollisionCombat object field


    private Rigidbody2D rb;
    private CapsuleCollider2D bc;
    private TrailRenderer tr;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;
    private InputAction m_DashAction;
    private InputAction m_DebugAction;


    private float playerHeight, playerWidth;
    // Whether player was on the ground in previous tick;  
    // used to figure out when to make dust particles
    private Direction direction = Direction.Right;
    private bool wasGrounded = false;
    
    // Timer which counts down after leaving ground, 
    // allowing "coyote-time" jumping  
    private float coyoteTimeCounter = 0f;
    // Timer which tracks how much longer the dash should last.
    private float dashTimeCounter = 0f;
    // Timer which tracks if the player can twirl.
    private float canTwirlTimeCounter = 0f;
    // Timer which tracks if jump is queued up.
    private float jumpBufferTimeCounter = 0f;
    // Assuming this is player input -Z
    private float moveHorizontal; 

    // Direction of the player when instatiating the dash
    private Direction dashDirection = Direction.Right;
    // How many more dashes can the player use before landing
    private int numDashesLeft; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<CapsuleCollider2D>();
        tr = GetComponent<TrailRenderer>();
        animator = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
        spriteRenderer = this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();

        playerHeight = bc.size.y;
        playerWidth = bc.size.x;

        numDashesLeft = numDashes;
        
        groundCheckBoxWidth = playerWidth;

        m_MoveAction = InputSystem.actions.FindAction("Player/Move");
        m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
        m_DashAction = InputSystem.actions.FindAction("Player/Dash");
        m_DebugAction = InputSystem.actions.FindAction("Player/Debug");

        m_MoveAction.Enable();
        m_JumpAction.Enable();
        m_DashAction.Enable();
        m_DebugAction.Enable();

        cc  = GameObject.Find("CollisionSystem").GetComponent<CollisionCombat>(); // initializes cc as the CollisionCombat script
    }
    
    void Update()
    {        
        bool isGrounded = IsGrounded();

        // Update timers:
        // While coyoteTimeCounter is positive, the player can jump
        // Timer is reset when player is on ground
        if (isGrounded) 
        {
            coyoteTimeCounter = coyoteTime;
        }
        else if (coyoteTimeCounter > 0f)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        // jumpBufferTimeCounter is positive when jump is queued
        if (jumpBufferTimeCounter > 0f)
            jumpBufferTimeCounter -= Time.deltaTime;
        // dashTimeCounter is positive when dash is occurring
        if (dashTimeCounter > 0f)
        {
            dashTimeCounter -= Time.deltaTime;
            // un-freeze gravity when dashing
            if (dashTimeCounter <= 0f)
                EndDash();
        }
        if (canTwirlTimeCounter > 0f)
            canTwirlTimeCounter -= Time.deltaTime;

        // Show dust particles when the player lands 
        if(!wasGrounded && isGrounded)
            dustParticleSystem.Play();
        // Reset dash count
        if (isGrounded)
            ResetDash();
        
        wasGrounded = isGrounded;

        // Handle horizontal movement
        if (dashTimeCounter <= 0f) 
        {
            // Determine direction player wants to face
            float moveHorizontal = m_MoveAction.ReadValue<Vector2>().x;
            if (moveHorizontal > 0f)
                direction = Direction.Right;
            else if (moveHorizontal < 0f)
                direction = Direction.Left;
            
            // Standard movement
            rb.linearVelocity = new Vector2(moveHorizontal * moveSpeed, rb.linearVelocity.y);
        }

        // if (m_JumpAction.IsPressed()) // this doesn't play well with the twirl + jump buffer
        if (m_JumpAction.WasPressedThisFrame())
            jumpBufferTimeCounter = jumpBufferTime;

        // Handle jumping / twirling
        if (jumpBufferTimeCounter > 0f)
        {
            if (coyoteTimeCounter > 0f)
                Jump();
            else if (canTwirlTimeCounter > 0f)
                StartTwirl();
        }

        // Handle dashing (/ twirling?)
        if (m_DashAction.WasPressedThisFrame())
        {
            if (numDashesLeft > 0)
                StartDash();
            else if (canTwirlTimeCounter > 0f)
                StartTwirl();
        }

        // You're not jumping if you're falling
        if (rb.linearVelocity.y < 0f)
            animator.SetBool("jumping", false);

        // Update always-set animator variables
        animator.SetBool("grounded", isGrounded);
        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x)); // should be named xSpeed technically but whatever
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        // Display sprite in correct direction
        bool flipSprite = (dashTimeCounter > 0f) ? (dashDirection == Direction.Left) : (direction == Direction.Left);
        spriteRenderer.flipX = flipSprite;

        // Debug to teleport to boss
        if (m_DebugAction.WasPressedThisFrame())
        {
            transform.position = new Vector2(358, 25f);
        }
    }

    void FixedUpdate()
    {
        
    }

    /// <summary>
    /// Checks whether there is a platform directly below the player object.
    /// </summary>
    bool IsGrounded()
    {
        /*
        Debug.DrawRay(transform.position, Vector2.down * (playerHeight / 2 + groundCheckDistance), Color.green);
        RaycastHit2D raycast = Physics2D.Raycast(
            transform.position, Vector2.down, playerHeight / 2 + groundCheckDistance, groundLayer);
        */
        
        Vector2 groundCheckBox = new Vector2(
            groundCheckBoxWidth, groundCheckBoxHeight
        );

        RaycastHit2D raycast = Physics2D.BoxCast(
            new Vector2(
                transform.position.x,
                transform.position.y - playerHeight / 2
            ),                      // origin
            groundCheckBox,         // size
            0,                      // angle
            Vector2.down,           // direction
            groundCheckBoxHeight,    // distance
            groundLayer             // layerMask
        );

        // Show BoxCast in debug
        BoxCastDrawer.Draw(
            raycast,            // hitInfo
            new Vector2(
                transform.position.x,
                transform.position.y - playerHeight / 2
            ),                  // origin
            groundCheckBox,     // size
            0,                  // angle
            Vector2.down,       // direction
            groundCheckBoxHeight // distance
        );
        
        /*if (raycast)
        {
            Debug.Log(Vector2.Dot(raycast.normal, Vector2.up));
            Debug.DrawLine(raycast.point, raycast.point + raycast.normal, Color.blue);
        }*/

        // Check if collided object surface is facing upwards
        return (raycast.collider != null) && (Vector2.Dot(raycast.normal, Vector2.up) > 0);
    }

    /// <summary>
    /// Make the player jump.
    /// </summary>
    void Jump()
    {
        // Reset coyote time & jump buffer immediately to prevent another jump
        coyoteTimeCounter = 0f; 
        jumpBufferTimeCounter = 0f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetBool("jumping", true);
        SoundManager.instance.PlaySoundEffect("jump");
        dustParticleSystem.Play();
    }

    /// <summary>
    /// Make the player dash.
    /// </summary>
    void StartDash()
    {
        // This shouldn't be a necessary condition, but just in case...
        if (numDashesLeft > 0)
        {
            tr.emitting = true;

            // Make player dash in the currently-facing direction
            dashDirection = direction;
            rb.linearVelocity = new Vector2(((int) dashDirection) * dashSpeed, 0f);

            canTwirlTimeCounter = canTwirlTime;
            dashTimeCounter = dashTime;
            animator.SetBool("dashing", true);
            SoundManager.instance.PlaySoundEffect("dash");
            // Freeze gravity while dashing
            rb.gravityScale = 0f; 
            numDashesLeft -= 1;
        }
    }

    void EndDash()
    {
        // Un-freeze gravity after dashing
        rb.gravityScale = gravityScale;
        animator.SetBool("dashing", false);
        dashTimeCounter = 0f;
        tr.emitting = false;
    }

    void ResetDash()
    {
        numDashesLeft = numDashes;
    }

    void StartTwirl()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, twirlSpeed);
        canTwirlTimeCounter = 0f;
        animator.SetBool("twirling", true);
        SoundManager.instance.PlaySoundEffect("twirl");
        twirlParticleSystem.Play();
        // TODO this should probably have an end twirl function somewhere
        animator.SetBool("twirling", false);

    }

    // This function needs 2 separate condition statements, since the Boss AI has it's own script apart from Enemy's
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("PlayerController OnCollisionEnter2D detected collision with " + col.gameObject.name);
        EntStats temp;
        CollisionCombat cs; // CollisionCombat object within this script
        bool isDashing = dashTimeCounter > 0f;
        //  on collision enter, the player should do damage to an enemy based on whether it's dashing or not
        if (col.gameObject.tag == "Enemy") // detecting collision as an enemy.
        {
            //cs = cc.gameObject.GetComponent<CollisionCombat>(); // Doing this to initialize cs as the script
            cc.CollisionEnemy(this.gameObject, col.gameObject, isDashing); // Player, Boss, Dash Check
        }
        if (col.gameObject.tag == "Boss") // detecting collision as a boss.
        {
            //cs = cc.gameObject.GetComponent<CollisionCombat>(); 
            cc.CollisionBoss(this.gameObject, col.gameObject, isDashing); // Player, Boss, Dash Check
        }


    }
    public void Animate()
    {
       /* bool isMoving;
        // This nest is checking if you're moving or not. 
        if(moveHorizontal > 0.1f || moveHorizontal < -0.1f)
            isMoving = true;
        else
        {
            isMoving = false;
        }
        */ // Commented out for now to avoid unnecessary warnings.
    }

    public void StopInput()
    {
        this.moveSpeed = 0; // Stops "this" object's move speed and not the others.
        // May need to add more here to stop jumping and dashing as well, but for now this is all we have. 
    }
}