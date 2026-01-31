using UnityEngine;

using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public LayerMask groundLayer;

    private Rigidbody2D rb;

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;


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
        
        // Handle jumping
        Debug.Log(m_JumpAction.WasPressedThisFrame() + " " + IsGrounded());
        if (m_JumpAction.WasPressedThisFrame() && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    
    bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector2.down, Color.green);
        RaycastHit2D raycast = Physics2D.Raycast(
            transform.position, Vector2.down, 1.25f, groundLayer);
        return raycast.collider != null;
    }
}