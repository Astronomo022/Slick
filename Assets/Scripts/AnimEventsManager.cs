using UnityEngine;

// This script is meant to be used for the animation events I placed within the animations of the Boss and elsewhere. 
// IMPORTANT: ANY FUNCTIONS IN THIS SCRIPT /MUST/ BE PUBLIC!
public class AnimEventsManager : MonoBehaviour
{
    private Animator anim; // collects the boss script from the object. 
    private BossAI boss; // this will collect the boss script from the parent. 
    
    #region BOSS
    void SendToFalling()
    {
        this.gameObject.GetComponent<Animator>().SetBool("isFalling", true);
        boss = this.gameObject.GetComponentInParent<BossAI>();
        //boss.StartFalling(); // calls the function in the boss script to make the boss fall.
    }

    void SendToSpotLoop()
    {
        // triggered from the "Crash" animation. Sends the boss to "Spotloop" state.
        // boss = this.gameObject.GetComponentInParent<BossAI>(); 
        this.gameObject.GetComponent<Animator>().SetBool("isOpen", true);
        
        
    } 

    void ResetToIdle()
    {
        // triggered from the "Close" animation. Resets the boss to idle, and resets the cycle. 
        this.gameObject.GetComponent<Animator>().SetTrigger("Idle_1");
        boss = this.gameObject.GetComponentInParent<BossAI>(); 
        // TODO: boss.SomeMethodToRaiseTheObjectBackUp();
    }
    #endregion
}
