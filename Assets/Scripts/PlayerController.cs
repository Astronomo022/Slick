using UnityEngine;

using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public float groundCheckDistance = 0.1f;
    public float groundCheckBoxWidth = 1.5f;

    // Defines which layers should be stood on
    public LayerMask groundLayer;
    // Which particles to play on jump or landing
    public ParticleSystem dustParticleSystem;

    private Rigidbody2D rb;
    private BoxCollider2D bc;

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;

    private float playerHeight, playerWidth;
    // Whether player was on the ground in previous tick; used 
    // to figure out when to make dust particles
    private bool wasGrounded = false;
    // Defines the object which 
    private Vector2 groundCheckBoxSize;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        playerHeight = bc.size.y;
        playerWidth = bc.size.x;
        groundCheckBoxSize = new Vector2(
            groundCheckBoxWidth, groundCheckDistance
        );

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
        // Show dust particles when the player lands 
        if(!wasGrounded && isGrounded)
        {
            dustParticleSystem.Play();
        }
        // Handle jumping
        if (m_JumpAction.IsPressed() && isGrounded)
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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        dustParticleSystem.Play();
    }
}