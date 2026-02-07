using Unity.VisualScripting;
using UnityEngine;

// Function that handles collision-based combat. 
public class CollisionCombat : MonoBehaviour
{
    private PlayerController pc;

    bool dead;
    // Arguments passed should be: the player, the enemy, and a bool representing a dash attack.
    public void CollisionReaction(GameObject player, EntStats enemy, bool dash)
    {
        EntStats jet = player.GetComponentInParent<EntStats>();
        EnemyAI eai = enemy.GetComponentInChildren<EnemyAI>();
        PlayerController pc = player.GetComponent<PlayerController>();
         if(dash)
           {
                dead = enemy.Damage(jet.damage);
                if(dead)
                {
                    eai.StopInput();
                    StartCoroutine(enemy.DeathAnim()); // wait for a few seconds for the fiery explode to finish
                    StopCoroutine(enemy.DeathAnim());

                }
                else
                {
                    StartCoroutine(enemy.HitAnim());
                    StopCoroutine(enemy.HitAnim());
                }
           }

           if(!dash)
           {

                dead = jet.Damage(enemy.damage);
                if(dead)
                {
                    pc.StopInput();
                    StartCoroutine(jet.DeathAnim()); // wait for a few seconds for the fiery explode to finish
                    StopCoroutine(jet.DeathAnim());

                }
                else
                {
                    StartCoroutine(jet.HitAnim());
                    StopCoroutine(jet.HitAnim());
                }
                    
           }

        
    }
// this is collision reaction but for the boss, since I decided to use a different script than for the boss than an enemy. 
    public void CollisionBoss(GameObject player, EntStats enemy, bool dash)
    {
        EntStats jet = player.GetComponentInParent<EntStats>();
        BossAI bai = enemy.GetComponentInChildren<BossAI>();
        PlayerController pc = player.GetComponent<PlayerController>();

        if(dash)
           {    
                if(bai.GetIsWeak())
                {
                    dead = enemy.Damage(jet.damage);
                    if(dead)
                    {
                        bai.StopInput();
                        StartCoroutine(enemy.DeathAnim()); // wait for a few seconds for the fiery explode to finish
                        StopCoroutine(enemy.DeathAnim());

                    }
                    else
                    {
                        StartCoroutine(enemy.HitAnim());
                        StopCoroutine(enemy.HitAnim());
                    }
                }
                else
                {
                    // Just this for right now, assuming in another script we have functionality where the player will simply bounce off 
                    return ; 
                }
           }

           if(!dash)
           {

            dead = jet.Damage(enemy.damage);
            if(dead)
            {
                pc.StopInput();
                StartCoroutine(jet.DeathAnim()); // wait for a few seconds for the fiery explode to finish
                StopCoroutine(jet.DeathAnim());

            }
            else
            {
                StartCoroutine(jet.HitAnim());
                StopCoroutine(jet.HitAnim());
            }
                
           }




        // Fill in from the todo above 
    }
}
