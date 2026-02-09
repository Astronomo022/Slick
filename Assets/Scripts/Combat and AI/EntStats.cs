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
        if(this.gameObject.tag == "Boss")
        {
            Collider2D bossCollider = this.gameObject.GetComponent<Collider2D>();
            //bossCollider.enabled = false; // disable the boss's collider
            anim.SetBool("IsHurt", true); // play hurt frame
            yield return new WaitForSecondsRealtime(0.3f); // may need to wait longer
            //bossCollider.enabled = true; // re-enable the boss's collider 
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
            Collider2D playerCollider = this.gameObject.GetComponentInChildren<Collider2D>();

            playerCollider.gameObject.SetActive(false); // disable the player's collider to hopefully prevent pushing

            //sfx.PlayPlayerDeath(); // commmented since we dont have an SFX manager yet, just to remove the errors
            yield return new WaitForSecondsRealtime(1.5f); // may need to wait longer
            
            
        }

        if(this.gameObject.tag == "Enemy")
        {
            // Same as the above in Player.

            
            Collider2D enemyCollider = this.gameObject.GetComponentInChildren<Collider2D>();

            //enemyCollider.enabled = false; // this isn't working for some reason
            anim = this.gameObject.GetComponentInChildren<Animator>(); 
            anim.SetBool("IsDead", true);
            SoundManager.instance.PlaySoundEffect("enemy_killed"); // Direct call to Sound Manager, may be a problem, but I'll allow it for now. 
            // Don't need a yield return here. The enemy will be destroyed by the AnimEventManager once the death animation is done, so we can just let it be. 

            // Usually we'd set the bool back to false, but the gameobject is destroyed, so doing so will cause an error. 


        }

        if(this.gameObject.tag == "Boss")
        {
            // Same as the above in Enemy, but for the boss. 
            
            BossAI bossAI = this.gameObject.GetComponent<BossAI>();
            Collider2D bossCollider = this.gameObject.GetComponentInChildren<Collider2D>();
            //Debug.Log("This enemy is: " + this.gameObject.name + "This collider is: " + bossCollider.gameObject.name);

            bossAI.StopInput();
            //bossCollider.gameObject.SetActive(false);
            anim = this.gameObject.GetComponentInChildren<Animator>(); 
            anim.SetBool("IsDead", true); // disable the boss's collider
            SoundManager.instance.PlaySoundEffect("enemy_killed");
            
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
