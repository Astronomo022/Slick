using UnityEngine;

using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private float groundCheckDistance = 0.1f;
    //public float groundCheckBoxWidth = 1.5f;
    [SerializeField] private float coyoteTime = 0.2f;

    // Defines which layers should be stood on
    [SerializeField] private LayerMask groundLayer;
    // Which particles to play on jump or landing
    [SerializeField] private ParticleSystem dustParticleSystem;

    private Rigidbody2D rb;
    private BoxCollider2D bc;

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;

    private float playerHeight, playerWidth;
    // Whether player was on the ground in previous tick;  
    // used to figure out when to make dust particles
    private bool wasGrounded = false;
    //private Vector2 groundCheckBoxSize;
    // Timer which counts down after leaving ground, 
    // allowing "coyote-time" jumping  
    private float coyoteTimeCounter = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        playerHeight = bc.size.y;
        playerWidth = bc.size.x;
        /*groundCheckBoxSize = new Vector2(
            groundCheckBoxWidth, groundCheckDistance
        );*/
        
        m_MoveAction = InputSystem.actions.FindAction("Player/Move");
        m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
        
        m_MoveAction.Enable();
        m_JumpAction.Enable();
    }
    
    void Update()
    {
        // Handle horizontal movement
        float moveHorizontal = m_MoveAction.ReadValue<Vector2>().x;
        rb.linearVelocity = new Vector2(moveHorizontal * moveSpeed, rb.linearVelocity.y);
        
        bool isGrounded = IsGrounded();
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
        Debug.Log(coyoteTimeCounter);

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
    }

    /// <summary>
    /// Checks whether there is a platform directly below the player object.
    /// </summary>
    bool IsGrounded()
    {
        //Debug.DrawRay(transform.position, Vector2.down * (playerHeight / 2 + groundCheckDistance), Color.green);
        RaycastHit2D raycast = Physics2D.Raycast(
            transform.position, Vector2.down, playerHeight / 2 + groundCheckDistance, groundLayer);
        
        /*
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
        */

        
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
}