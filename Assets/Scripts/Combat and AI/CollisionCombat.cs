using Unity.VisualScripting;
using UnityEngine;

// Function that handles collision-based combat. 
public class CollisionCombat : MonoBehaviour
{
    private PlayerController pc;

    bool dead;
    /// <summary>
    /// Call appropriate functions when the player collides with an enemy. 
    /// </summary>
    /// <param name="player">The player GameObject.</param>
    /// <param name="enemy">The enemy colliding with the player.</param>
    /// <param name="dash">Whether the player is currently dashing.</param>
    public void CollisionEnemy(GameObject player, GameObject enemy, bool dash)
    {
        //Debug.Log("CollisionEnemy function called in CollisionCombat script.");
        PlayerController pc = player.GetComponent<PlayerController>();
        EnemyAI eai = enemy.GetComponent<EnemyAI>();

        EntStats playerEntityStats = player.GetComponent<EntStats>();
        EntStats enemyEntityStats = enemy.GetComponent<EntStats>();

        if (dash)
        {
            dead = enemyEntityStats.Damage(playerEntityStats.damage);
            if (dead)
            {
                Debug.Log("Enemy has been killed by the player.");
                eai.StopInput();
                StartCoroutine(enemyEntityStats.DeathAnim()); // wait for a few seconds for the fiery explode to finish
                StopCoroutine(enemyEntityStats.DeathAnim());
            }
            else
            {
                StartCoroutine(enemyEntityStats.HitAnim());
                StopCoroutine(enemyEntityStats.HitAnim());
            }
        }

        if (!dash)
        {
            dead = playerEntityStats.Damage(enemyEntityStats.damage);
            if (dead)
            {
                pc.StopInput();
                StartCoroutine(playerEntityStats.DeathAnim()); // wait for a few seconds for the fiery explode to finish
                StopCoroutine(playerEntityStats.DeathAnim());

            }
            else
            {
                StartCoroutine(playerEntityStats.HitAnim());
                StopCoroutine(playerEntityStats.HitAnim());
            }
        }
    }

    // this is collision reaction but for the boss, since I decided to use a different script than for the boss than an enemy. 
    /// <summary>
    /// Call appropriate functions when the player collides with the boss. 
    /// </summary>
    /// <param name="player">The player GameObject.</param>
    /// <param name="enemy">The boss colliding with the player.</param>
    /// <param name="dash">Whether the player is currently dashing.</param>
    public void CollisionBoss(GameObject player, GameObject boss, bool dash)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        BossAI bai = boss.GetComponent<BossAI>();

        EntStats playerEntityStats = player.GetComponent<EntStats>();
        EntStats bossEntityStats = boss.GetComponent<EntStats>();

        Debug.Log(pc + " " + bai + " " + playerEntityStats + " " + bossEntityStats);

        if (dash)
        {
            if (bai.GetIsWeak())
            {
                dead = bossEntityStats.Damage(playerEntityStats.damage);
                if (dead)
                {
                    bai.StopInput();
                    StartCoroutine(bossEntityStats.DeathAnim()); // wait for a few seconds for the fiery explode to finish
                    StopCoroutine(bossEntityStats.DeathAnim());
                }
                else
                {
                    StartCoroutine(bossEntityStats.HitAnim());
                    StopCoroutine(bossEntityStats.HitAnim());
                }
            }
            else
            {
                // Just this for right now, assuming in another script we have functionality where the player will simply bounce off 
                return;
            }
        }

        if (!dash)
        {

            dead = playerEntityStats.Damage(bossEntityStats.damage);
            if (dead)
            {
                pc.StopInput();
                StartCoroutine(playerEntityStats.DeathAnim()); // wait for a few seconds for the fiery explode to finish
                StopCoroutine(playerEntityStats.DeathAnim());

            }
            else
            {
                StartCoroutine(playerEntityStats.HitAnim());
                StopCoroutine(playerEntityStats.HitAnim());
            }

        }

        // Fill in from the todo above 
    }
}
