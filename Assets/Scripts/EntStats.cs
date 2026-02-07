using UnityEngine;
using System.Collections;

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
        anim.SetBool("IsHit", true);
        if (this.gameObject.tag == "Player" )
        {
            anim.SetTrigger("Hurt");
            //sfx.PlayPlayerHurt(); // commmented since we dont have an SFX manager yet, just to remove the errors
        }
        

        yield return new WaitForSecondsRealtime(0.3f);
        anim.SetBool("IsHit", false);
    }

    public IEnumerator DeathAnim()
    {
        anim = this.gameObject.GetComponentInChildren<Animator>(); // must be done, since type is private in here.
        if(this.gameObject.tag == "Player")
        {
            // Technically, this conditional can go unused for this jam, but were we to add more, we'd set up a system for gameovers and what not. 

            //sfx.PlayPlayerDeath(); // commmented since we dont have an SFX manager yet, just to remove the errors
            yield return new WaitForSecondsRealtime(1.5f); // may need to wait longer
            
        }

        if(this.gameObject.tag == "Enemy" || this.gameObject.tag == "Boss")
        {
            //sfx.PlayEnemyDeath(); // commmented since we dont have an SFX manager yet, just to remove the errors
            anim.SetTrigger("Hurt");
            yield return new WaitForSecondsRealtime(0.5f); // may need to wait longer
        }
        yield return new WaitForSecondsRealtime(1.3f); // may need to wait longer or replace with NULL
        Destroy(this.gameObject); // Calls this function to delete the object after yield return. 
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
