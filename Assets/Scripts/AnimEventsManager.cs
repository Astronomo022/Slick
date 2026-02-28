using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


// This script is meant to be used for the animation events I placed within the animations of the Boss and elsewhere. 
// IMPORTANT: ANY FUNCTIONS IN THIS SCRIPT /MUST/ BE PUBLIC!
public class AnimEventsManager : MonoBehaviour
{
    private Animator anim; // collects the boss script from the object. 
    private BossAI boss; // this will collect the boss script from the parent. 
    
    #region BOSS
    void SendToFalling()
    {
        bool temp = true;
        //Debug.Log(this.gameObject.name+ " and " + this.gameObject.tag);
        boss = this.gameObject.GetComponentInParent<BossAI>();
        boss.isFalling = temp; // Probably not a good solution

        // Reasoning the above didn't work at first: Because I didn't put brackets around an if statement.

        StartCoroutine(boss.StartFalling()); // calls the function in the boss script to make the boss fall.
        StopCoroutine(boss.StartFalling());
        //boss.StartFalling(); // calls the function in the boss script to make the boss fall.
    }

    void SendToSpotLoop()
    {
        // triggered from the "Crash" animation. Sends the boss to "Spotloop" state.
        boss = this.gameObject.GetComponentInParent<BossAI>(); 
        boss.isOpen = true;
        StartCoroutine(boss.StartWeakTimer());
        StopCoroutine(boss.StartWeakTimer());
        
        
        
    } 

    void ResetToIdle()
    {
        // triggered from the "Close" animation. Resets the boss to idle, and resets the cycle. 
        //this.gameObject.GetComponent<Animator>().SetTrigger("Idle_1");
        boss = this.gameObject.GetComponentInParent<BossAI>(); 
        StartCoroutine(boss.StartRising()); // calls the function in the boss script to make the boss rise back up.
        StopCoroutine(boss.StartRising());
        // TODO: boss.SomeMethodToRaiseTheObjectBackUp();
    }
    
    #endregion

    void Destroy()
    {
        // We do it this way, because in the past, when I tried doing it in one line, a NullReferneceException would occur. 
        Debug.Log("Destroy function called in AnimEventsManager script.");
        Rigidbody2D original = this.gameObject.GetComponentInParent<Rigidbody2D>(); // GetComponent<> should work too, but this is fine. 
        Destroy(original.gameObject); // Destoys the gameobject the assigned rb was attached to. 
    }
}
