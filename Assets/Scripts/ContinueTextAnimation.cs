using UnityEngine;
using System.Collections;
public class ContinueTextAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BobAnimation());
    }

    IEnumerator BobAnimation()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.anchoredPosition;
        float bobHeight = 10f;
        float bobSpeed = 2f;

        while (true)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            rectTransform.anchoredPosition = new Vector3(startPosition.x, newY, startPosition.z);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
