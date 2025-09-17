using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator FadeOut(float duration)
    {
        Color c = fadeImage.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0, 1, t / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1;
        fadeImage.color = c;
    }

    public IEnumerator FadeIn(float duration)
    {
        Color c = fadeImage.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(1, 0, t / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0;
        fadeImage.color = c;
    }
}
