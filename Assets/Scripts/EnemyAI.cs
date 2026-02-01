using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // class variables
    private Rigidbody2D rb; // the enemy's rb, specifically.
    private Animator anim; // direct reference to our animator for proper animation switching on detection
    public Transform player; // This will ref our player object's transform

    // optional boolean flags for future work
    // TODO: canDash boolean for when we want smarter AI behavior, to dash into us, if we can work it in
    
    // primitive data types 
    public float chaseSpeed; // as always... Self explanatory. 
    float direction;
    //float elevation; // may not be needed in this project, but here just in case. 

    bool detection; // whether the enemy has detected the player or not.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        AIChase();
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

    void EnemyAnimate()
    {
        // Placeholder for future animation code
    }

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
}
