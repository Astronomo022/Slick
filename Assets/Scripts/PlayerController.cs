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
    [SerializeField] private float groundCheckBoxWidth = 1f;

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


    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private TrailRenderer tr;

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;
    private InputAction m_DashAction;


    private float playerHeight, playerWidth;
    // Whether player was on the ground in previous tick;  
    // used to figure out when to make dust particles
    private Direction direction;
    private bool wasGrounded = false;
    
    // Timer which counts down after leaving ground, 
    // allowing "coyote-time" jumping  
    private float coyoteTimeCounter = 0f;
    // Timer which tracks how much longer the dash should last.
    private float dashTimeCounter = 0f;
    // Timer which tracks if the player can twirl.
    private float canTwirlTimeCounter = 0f;

    // Direction of the player when instatiating the dash
    private Direction dashDirection = Direction.Right;
    // How many more dashes can the player use before landing
    private int numDashesLeft; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        tr = GetComponent<TrailRenderer>();

        playerHeight = bc.size.y;
        playerWidth = bc.size.x;

        numDashesLeft = numDashes;
        
        m_MoveAction = InputSystem.actions.FindAction("Player/Move");
        m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
        m_DashAction = InputSystem.actions.FindAction("Player/Dash");
        m_MoveAction.Enable();
        m_JumpAction.Enable();
        m_DashAction.Enable();
    }
    
    void Update()
    {        
        bool isGrounded = IsGrounded();

        // Update timers:
        // While coyoteTimeCounter is positive, the player can jump
        // Timer is reset when player is on ground
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else if (coyoteTimeCounter > 0f)
            coyoteTimeCounter -= Time.deltaTime;
        // dashTimeCounter is positive when dash is occurring
        if (dashTimeCounter > 0f)
            dashTimeCounter -= Time.deltaTime;
        if (canTwirlTimeCounter > 0f)
            canTwirlTimeCounter -= Time.deltaTime;

        // Show dust particles when the player lands 
        if(!wasGrounded && isGrounded)
            dustParticleSystem.Play();
        // Reset dash count
        if (isGrounded)
            ResetDash();
        
        wasGrounded = isGrounded;

        // un-freeze gravity when dashing
        if (dashTimeCounter <= 0f)
            EndDash();

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

        // Handle jumping / twirling
        if (m_JumpAction.IsPressed())
        {
            if (coyoteTimeCounter > 0f)
                Jump();
            else if (canTwirlTimeCounter > 0f)
                Twirl();
        }

        // Handle dashing (/ twirling?)
        if (m_DashAction.WasPressedThisFrame())
        {
            if (numDashesLeft > 0)
                StartDash();
            else if (canTwirlTimeCounter > 0f)
                Twirl();
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
        // Reset coyote time immediately to prevent another jump
        coyoteTimeCounter = 0f; 
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
            rb.gravityScale = 0f; 
            tr.emitting = true;

            // Make player dash in the currently-facing direction
            dashDirection = direction;
            rb.linearVelocity = new Vector2(((int) dashDirection) * dashSpeed, 0f);

            canTwirlTimeCounter = canTwirlTime;
            dashTimeCounter = dashTime;
            numDashesLeft -= 1;
        }
    }

    void EndDash()
    {
        // Un-freeze gravity after dashing
        rb.gravityScale = gravityScale;
        tr.emitting = false;
    }

    void ResetDash()
    {
        numDashesLeft = numDashes;
    }

    void Twirl()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, twirlSpeed);
        canTwirlTimeCounter = 0f;
        twirlParticleSystem.Play();
    }
}