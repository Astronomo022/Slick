using UnityEngine;

// This class is basically a copy of EnemyAI and then some. 
// Also copies from PlayerController for stuff like isGrounded

public class BossAI : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer; // Copied from playercontroller for IsGrounded function.
    [SerializeField] private float groundCheckBoxWidth, groundCheckBoxHeight; // Copied from playercontroller for IsGrounded function.
    private Rigidbody2D rb; // the enemy's rb, specifically.
    private BoxCollider2D bc;
    private Animator anim; // direct reference to our animator for proper animation switching on detection
    public Transform player; // This will ref our player object's transform

    // optional boolean flags for future work
    // TODO: canDash boolean for when we want smarter AI behavior, to dash into us, if we can work it in
    
    // primitive data types 
    public float chaseSpeed; // as always... Self explanatory. 
    private float bossHeight, bossWidth;
    float direction;
    //float elevation; // may not be needed in this project, but here just in case. 

    bool detection; // whether the enemy has detected the player or not.
    bool isGrounded;
    private bool isWeak;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        //tr = GetComponent<TrailRenderer>();

        bossHeight = bc.size.y;
        bossWidth = bc.size.x;
    }
    void Start()
    {
        isGrounded = IsGrounded();
        isWeak = false;
    }

    // Update is called once per frame
    void Update()
    {   
        isGrounded = IsGrounded(); // returns true if the boss is grounded, false if not.
        AIChase();
        BossAnimate();
    }

    void AIChase()
    {
        if(detection)
        {
            if(player == null) // defensive programming at it's finest 
            {
                detection = false;
                return;
            }
        // Mathf.Sign may not need to be here, we can remove if need be. 
        direction = Mathf.Sign(player.position.x - transform.position.x);

        // should move the rb in the direction of the player. Needs to be tested. 
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y); 
        }
    }

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
                transform.position.y - bossHeight / 2
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
                transform.position.y - bossHeight / 2
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

    void BossAnimate()
    {
        // Placeholder for future animation code
        bool isMoving;
        // This nest is checking if you're moving or not. 
        if(direction > 0.1f || direction < -0.1f) 
            isMoving = true;
        else
        {
            isMoving = false;
        }
        if(isGrounded)
            anim.SetBool("isFalling", false); 
        
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("X",direction);
        anim.SetBool("isWeak", isWeak);
    }

    // This function automatically runs if a gameobject's tag is named with "Player"
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            detection = true;

        } 
    }
    
    public void StopInput()
    {
        this.chaseSpeed = 0; // Stops "this" object's chase speed and not the others.
        detection = false; // Stops chasing the player.

    }
    public bool GetIsWeak()
    {
        return this.isWeak;
    }

}
