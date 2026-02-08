using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // class variables
    private Rigidbody2D rb; // the enemy's rb, specifically.
    private Animator animator; // direct reference to our animator for proper animation switching on detection
    [SerializeField] private Transform player; // This will ref our player object's transform
    private SpriteRenderer sr;

    // optional boolean flags for future work
    // TODO: canDash boolean for when we want smarter AI behavior, to dash into us, if we can work it in
    
    // primitive data types 
    [SerializeField] private float chaseSpeed; // as always... Self explanatory. 
    [SerializeField] private float playerTrackingDeadZone = 1; // horizontal region in which the enemy will not target the player
    // intended to stop flickering behavior 
    float direction;
    //float elevation; // may not be needed in this project, but here just in case. 

    [Header("Debug")]
    [SerializeField] private bool detection; // whether the enemy has detected the player or not.
    // I've set this to a SerializedField so I can toggle it for testing purposes.
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Transform spriteObject = this.gameObject.transform.GetChild(0);
        sr = spriteObject.GetComponent<SpriteRenderer>();
        animator = spriteObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        AIChase();
        EnemyAnimate();
    }

    void AIChase()
    {
        if(detection)
        {
            if(player == null) // Defensive programming at its finest 
            {
                detection = false;
                return;
            }
            // Mathf.Sign may not need to be here, we can remove if need be. 
            float playerPositionDelta = player.position.x - transform.position.x;
            if(Mathf.Abs(playerPositionDelta) >= playerTrackingDeadZone)
            {
                direction = Mathf.Sign(playerPositionDelta);
                // Should move the rb in the direction of the player. Needs to be tested. 
                rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y); 
            }
        }
    }

    void EnemyAnimate()
    {
        // Placeholder for future animation code
        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        // To Zombie: I don't know if this is how you wanted to do this, but it should work
        sr.flipX = !(rb.linearVelocity.x < 0);
        // I know this could be done simpler, but I wanted to specifically show that 
        // the sprites are drawn flipped relative to the player's sprites
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Enemy trigger with object " + col.gameObject.tag + ", name " + col.gameObject.name);
        if(col.gameObject.name == player.gameObject.name)
        {
            detection = true;
        }
    }

    public void StopInput()
    {
        chaseSpeed = 0;
        detection = false; // Stops chasing the player.

    }
}
