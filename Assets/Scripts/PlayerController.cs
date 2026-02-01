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

    [SerializeField] private float moveSpeed = 5f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    public float groundCheckBoxWidth = 0.99f;
    [SerializeField] private float coyoteTime = 0.2f;

    [Header("Dashing")]
    [SerializeField] private int numDashes = 1; 
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private float dashTime = 0.5f;

    [Header("Components")]
    // Defines which layers should be stood on
    [SerializeField] private LayerMask groundLayer;
    // Which particles to play on jump or landing
    [SerializeField] private ParticleSystem dustParticleSystem;

    private Rigidbody2D rb;
    private BoxCollider2D bc;

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;
    private InputAction m_DashAction;


    private float playerHeight, playerWidth;
    // Whether player was on the ground in previous tick;  
    // used to figure out when to make dust particles
    private Direction direction;
    private bool wasGrounded = false;
    // Width of the area under the player to check for grounded-ness
    private Vector2 groundCheckBoxSize;
    // Timer which counts down after leaving ground, 
    // allowing "coyote-time" jumping  
    private float coyoteTimeCounter = 0f;
    // Timer which tracks how much longer the dash should last.
    private float dashTimeCounter = 0f;
    // Direction of the player when instatiating the dash
    private Direction dashDirection = Direction.Right;
    // How many more dashes can the player use before landing
    private int numDashesLeft; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        playerHeight = bc.size.y;
        playerWidth = bc.size.x;
        numDashesLeft = numDashes;
        
        groundCheckBoxSize = new Vector2(
            groundCheckBoxWidth, groundCheckDistance
        );

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
        // While coyoteTimeCounter is above zero, jump has been recently pressed;
        // character will jump at the next possible opportunity
        if (coyoteTimeCounter > 0f)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if(isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        // dashTimeCounter is positive when dash is occurring
        if (dashTimeCounter > 0f)
        {
            dashTimeCounter -= Time.deltaTime;
        }

        // Show dust particles when the player lands 
        if(!wasGrounded && isGrounded)
        {
            dustParticleSystem.Play();
        }
        // Handle jumping
        if (m_JumpAction.IsPressed() && coyoteTimeCounter > 0f)
        {
            Jump();
        }
        wasGrounded = isGrounded;
        // Reset dash
        if (isGrounded)
        {
            numDashesLeft = numDashes;
        }

        // Freeze gravity when dashing
        if (dashTimeCounter > 0f)
        {
            rb.gravityScale = 0; 
        } 
        else
        {
            rb.gravityScale = gravityScale; 
        }

        // Handle horizontal movement
        if (dashTimeCounter > 0f) 
        {
            // Continue dash while dash time counter hasn't yet run out
            rb.linearVelocity = new Vector2(((int) dashDirection) * dashSpeed, rb.linearVelocity.y);
        }
        else 
        {
            // Standard movement
            float moveHorizontal = m_MoveAction.ReadValue<Vector2>().x;
            if (moveHorizontal > 0f) { direction = Direction.Right; } 
            else if (moveHorizontal < 0f) { direction = Direction.Left; }
            rb.linearVelocity = new Vector2(moveHorizontal * moveSpeed, rb.linearVelocity.y);

            // Dashing
            if (m_DashAction.WasPressedThisFrame() && numDashesLeft > 0)
            {
                Dash();
            }
        }
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
        
        RaycastHit2D raycast = Physics2D.BoxCast(
            new Vector2(
                transform.position.x,
                transform.position.y - playerHeight / 2
            ),                      // origin
            groundCheckBoxSize,     // size
            0,                      // angle
            Vector2.down,           // direction
            groundCheckDistance,    // distance
            groundLayer             // layerMask
        );

        BoxCastDrawer.Draw(
            raycast,            // hitInfo
            new Vector2(
                transform.position.x,
                transform.position.y - playerHeight / 2
            ),                  // origin
            groundCheckBoxSize, // size
            0,                  // angle
            Vector2.down,       // direction
            groundCheckDistance // distance
        );
        
        return raycast.collider != null;
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
    void Dash()
    {
        // This shouldn't be a necessary condition, but just in case...
        if (numDashesLeft > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            dashDirection = direction;
            dashTimeCounter = dashTime;
            numDashesLeft -= 1;
        }
    }
}