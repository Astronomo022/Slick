using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;

// NOTICE: This script is a copy and paste of LogoAnimation, allocated to slide in from right instead of left, and some additional tweaks. 

public class CreditsAnimation : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SlideInFromRight());
    }

    public IEnumerator SlideInFromRight()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        // In a stark difference to the Logo Anim, we require screen width to be multiplied in order to come from the right.
        Vector2 startPos = new Vector2(1.7f * Screen.width, rectTransform.anchoredPosition.y); // need to start just outside right screen. 
        Vector2 endPos = rectTransform.anchoredPosition;
        
        float duration = 1f;
        float elapsed = 0f;
        
        rectTransform.anchoredPosition = startPos;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        
        rectTransform.anchoredPosition = endPos;
        
        // Gonna do a series here to enable bobbing.
        GameObject credit = this.gameObject;
        credit.GetComponent<ContinueTextAnimation>().enabled = true;
        
        //SoundManager.instance.PlaySoundEffect("title_call"); // not necessary, since title script calls this already. 

    }

    
    public IEnumerator SlideOutToRight()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(1.7f * Screen.width, rectTransform.anchoredPosition.y);
        
        float duration = 1f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        
        rectTransform.anchoredPosition = endPos;
    }
}
