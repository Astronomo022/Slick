using UnityEngine;

using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    // Defines which layers should be stood on
    public LayerMask groundLayer;
    // Which particles to play on jump or landing
    public ParticleSystem dustParticleSystem;

    private Rigidbody2D rb;

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;

    // Whether player was on the ground in previous tick; used 
    // to figure out when to make dust particles
    private bool wasGrounded = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

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
        if (m_JumpAction.WasPressedThisFrame() && isGrounded)
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
        //Debug.DrawRay(transform.position, Vector2.down, Color.green);
        RaycastHit2D raycast = Physics2D.Raycast(
            transform.position, Vector2.down, 1.25f, groundLayer);
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