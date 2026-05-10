using Unity.VisualScripting;
using UnityEngine;

// Function that handles collision-based combat. 
public class CollisionCombat : MonoBehaviour
{
    private PlayerController pc;

    /// <summary>
    /// Call appropriate functions when the player collides with an enemy. 
    /// </summary>
    /// <param name="player">The player GameObject.</param>
    /// <param name="enemy">The enemy colliding with the player.</param>
    /// <param name="isPlayerDashing">Whether the player is currently dashing.</param>
    public void CollisionEnemy(GameObject player, GameObject enemy, bool isPlayerDashing)
    {
        //Debug.Log("CollisionEnemy function called in CollisionCombat script.");
        PlayerController pc = player.GetComponent<PlayerController>();
        EnemyAI eai = enemy.GetComponent<EnemyAI>();

        EntStats playerEntityStats = player.GetComponent<EntStats>();
        EntStats enemyEntityStats = enemy.GetComponent<EntStats>();
        bool isEnemyDead, isPlayerDead; // booleans to check if either entity is dead after the damage is applied.

        if (isPlayerDashing)
        {
            isEnemyDead = enemyEntityStats.Damage(playerEntityStats.damage);
            if (isEnemyDead)
            {
                Debug.Log("Enemy has been killed by the player.");
                // TODO: Move to its own method
                enemy.GetComponent<Collider2D>().enabled = false; // disable the enemy's collider to hopefully prevent pushing after death
                enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; // stop the enemy's movement immediately after death
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

        if (!isPlayerDashing)
        {
            isPlayerDead = playerEntityStats.Damage(enemyEntityStats.damage);
            player.GetComponent<PlayerController>().BouncePlayer(14f); // bounce the player up with a force of 10, may need to be tweaked for better feel.
            if (isPlayerDead)
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
        bool isEnemyDead, isPlayerDead; // booleans to check if either entity is dead after the damage is applied.

        //Debug.Log(pc + " " + bai + " " + playerEntityStats + " " + bossEntityStats);

        if (dash)
        {
            if (bai.GetIsWeak())
            {
                isEnemyDead = bossEntityStats.Damage(playerEntityStats.damage);
                if (isEnemyDead)
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

            isPlayerDead = playerEntityStats.Damage(bossEntityStats.damage);
            if (isPlayerDead)
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
