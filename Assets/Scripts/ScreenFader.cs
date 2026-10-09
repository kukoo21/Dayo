using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    private static ScreenFader instance;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // keep across scenes/maps
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // start transparent
    }

    public static IEnumerator FadeOut(float duration)
    {
        if (instance == null) yield break;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            instance.canvasGroup.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }

        instance.canvasGroup.alpha = 1f; // ensure fully black
    }

    public static IEnumerator FadeIn(float duration)
    {
        if (instance == null) yield break;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            instance.canvasGroup.alpha = 1f - Mathf.Clamp01(t / duration);
            yield return null;
        }

        instance.canvasGroup.alpha = 0f; // ensure fully clear
    }
}
