using UnityEngine;
using System.Collections;

public class LogoAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SlideInFromLeft());
    }

    public IEnumerator SlideInFromLeft()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 startPos = new Vector2(-Screen.width, rectTransform.anchoredPosition.y);
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
        SoundManager.instance.PlaySoundEffect("title_call");

    }

    public IEnumerator SlideOutToLeft()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(-Screen.width, rectTransform.anchoredPosition.y);
        
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
