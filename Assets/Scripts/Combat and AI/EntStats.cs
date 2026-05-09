using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class EntStats : MonoBehaviour
{
    public int damage, maxHP, curHP;
    private Animator anim;
    //public SFXManager sfx; // reference to the sfx manager for hit sounds.

    

    public bool Damage(int dmg)
    {
        curHP -= dmg;
        //Debug.Log("Damage is: " + dmg + " Current HP is: " + curHP); // Debug log to check if damage is being applied correctly.
        if (curHP <= 0)
        {
            curHP = 0;
            return true; // Entity is dead
        }
        return false; // Entity is still alive
    }

    public IEnumerator HitAnim()
    {
        anim = this.gameObject.GetComponentInChildren<Animator>(); // must be done, since type is private in here. 

        if(this.gameObject.tag == "Player")
        {
            //sfx.PlayPlayerHit(); // commmented since we dont have an SFX manager yet, just to remove the errors
            Collider2D playerCollider = this.gameObject.GetComponent<Collider2D>();

            //playerCollider.enabled = false; // disable the player's collider to hopefully prevent pushing

            anim.SetBool("IsHurt", true); // play hurt frame
            yield return new WaitForSecondsRealtime(0.3f); // may need to wait longer
            //playerCollider.enabled = true; // re-enable the player's collider 
            anim.SetBool("IsHurt", false); // set to false so the exit transition triggers for animation
        }

        if(this.gameObject.tag == "Boss")
        {
            //Collider2D bossCollider = this.gameObject.GetComponent<Collider2D>();
            //Rigidbody2D bossRB = this.gameObject.GetComponent<Rigidbody2D>();
            //bossRB.simulated = false; // disable the boss's physics to hopefully prevent pushing
            //bossCollider.enabled = false; // disable the boss's collider
            anim.SetBool("IsHurt", true); // play hurt frame
            yield return new WaitForSecondsRealtime(0.3f); // may need to wait longer
            //bossCollider.enabled = true; // re-enable the boss's collider 
            //bossRB.simulated = true; // re-enable the boss's physics
            anim.SetBool("IsHurt", false); // set to tfalse so the exit transition triggers for animation
        }
       
        

        yield return null; // I wrote the bottom one first, but this is here to return nothing. 
        
    }

    public IEnumerator DeathAnim()
    {
        anim = this.gameObject.GetComponentInChildren<Animator>(); // must be done, since type is private in here.
        if(this.gameObject.tag == "Player")
        {
            // Technically, this conditional can go unused for this jam, but were we to add more, we'd set up a system for gameovers and what not. 
            
            // This declaration so we can fetch Collider to prevent more collisions. 
            Collider2D playerCollider = this.gameObject.GetComponent<Collider2D>();
            PlayerController pc = this.gameObject.GetComponent<PlayerController>();

            //playerCollider.gameObject.SetActive(false); // disable the player's collider to hopefully prevent pushing
            pc.StopInput(); // stop player input upon death
            anim.SetBool("IsHurt", true);
            yield return new WaitForSecondsRealtime(0.2f);
            anim.SetBool("IsHurt", false);
            anim.SetBool("IsDead", true);

            //sfx.PlayPlayerDeath(); // commmented since we dont have an SFX manager yet, just to remove the errors
            yield return new WaitForSecondsRealtime(1.6f);
            SceneManager.LoadScene("FirstLevel"); // reloads the scene, effectively "respawning" the player.
            
            
        }

        if(this.gameObject.tag == "Enemy")
        {
            // Same as the above in Player.

            
            Collider2D enemyCollider = this.gameObject.GetComponentInChildren<BoxCollider2D>(); // Get the collider of the enemy, which is on the child object.
            //Debug.Log("Gameobject name: "+ enemyCollider.gameObject.name);
            //enemyCollider.gameObject.SetActive(false); // this isn't working for some reason
            enemyCollider.gameObject.layer = LayerMask.NameToLayer("DeadEnemy"); // This so that the collider child doesn't interact
            this.gameObject.layer = LayerMask.NameToLayer("DeadEnemy"); // set the enemy to a layer that won't interact with the player, since disabling the collider isn't working for some reason.
            anim.SetBool("IsDead", true);
            SoundManager.instance.PlaySoundEffect("enemy_killed"); // Direct call to Sound Manager, may be a problem, but I'll allow it for now. 
            // Don't need a yield return here. The enemy will be destroyed by the AnimEventManager once the death animation is done, so we can just let it be. 

            // Usually we'd set the bool back to false, but the gameobject is destroyed, so doing so will cause an error. 


        }

        if(this.gameObject.tag == "Boss")
        {
            // Same as the above in Enemy, but for the boss. 
            Debug.Log("Went through deathanim"); // debugging since it seems anim is not transitioning properly.
            BossAI bossAI = this.gameObject.GetComponent<BossAI>();
            //Collider2D bossCollider = this.gameObject.GetComponent<Collider2D>();// Solution here may be to just make the enemies rb kinematic, but we'll see.
            //Debug.Log("This enemy is: " + this.gameObject.name + "This collider is: " + bossCollider.gameObject.name);

            bossAI.StopInput();
            anim.SetBool("IsHurt", true);
            yield return new WaitForSecondsRealtime(0.3f);  
            //bossCollider.gameObject.SetActive(false);
            anim.SetBool("IsDead", true); // disable the boss's collider
            SoundManager.instance.PlaySoundEffect("enemy_killed");

            // This next series NEEDS to be changed, should there be more than one boss, this is just for now. 
            yield return new WaitForSecondsRealtime(1.2f);// Give the player time to process.
            SceneManager.LoadScene("CreditsMenu"); // Load the credits menu after the boss dies. 
            
        }

        yield return null; // Here so we can have something to return, even if it's nothing, since not all conditions return a value. 

        /* This entire condition needs to be reworked. The enemies that die, stay active for a while,
        repeat a sound, then explode if the animation has it. This is not proper, and can be done better if we simply have a
        transition for IsDead to trigger. The transition then happens immediately, of which we can use AnimEventManager to call 
        Destroy once the animation is done.

        if(this.gameObject.tag == "Enemy" || this.gameObject.tag == "Boss")
        {
            //sfx.PlayEnemyDeath(); // commmented since we dont have an SFX manager yet, just to remove the errors
            //anim.SetTrigger("Hurt");
            SoundManager.instance.PlaySoundEffect("boss_falling");
            anim.SetBool("IsHurt", true);
            //yield return new WaitForSecondsRealtime(0.5f); // may need to wait longer
        
            yield return new WaitForSecondsRealtime(1f); // may need to wait longer or replace with NULL
            anim.SetTrigger("Explode");
            SoundManager.instance.PlaySoundEffect("enemy_killed");
            yield return new WaitForSecondsRealtime(0.3f); // may need to wait longer or replace with NULL
            SoundManager.instance.PlaySoundEffect("enemy_killed");
            yield return new WaitForSecondsRealtime(0.3f); // may need to wait longer or replace with NULL
            SoundManager.instance.PlaySoundEffect("enemy_killed");
            yield return new WaitForSecondsRealtime(0.1f); // may need to wait longer
            Destroy(this.gameObject); // Calls this function to delete the object after yield return. 
        } 
        */
        
    }

    public void Heal(int regenRate)
    {

        curHP += regenRate;
        if (curHP > maxHP)
        {
            curHP = maxHP;
            
        }
        
    }
}
