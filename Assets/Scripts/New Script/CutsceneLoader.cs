using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneLoader : MonoBehaviour
{
    public Image fadeImage;             // Black full-screen Image
    public Text cutsceneText;           // UI Text ("Welcome, Dayo")
    public float fadeDuration = 1.0f;   // Time for fade in/out
    public float showDuration = 2.0f;   // Time text stays visible

    private void Start()
    {
        // Make sure background starts black
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f; // fully opaque black
            fadeImage.color = c;
        }

        // Make sure text starts invisible
        if (cutsceneText != null)
        {
            Color t = cutsceneText.color;
            t.a = 0f;
            cutsceneText.color = t;
        }

        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // 1. Fade text in (background stays black)
        yield return StartCoroutine(FadeText(0f, 1f));

        // 2. Hold text
        yield return new WaitForSeconds(showDuration);

        // 3. Fade text out
        yield return StartCoroutine(FadeText(1f, 0f));

        // 4. Fade background again (for smoothness)
        yield return StartCoroutine(FadeImage(1f, 1f)); // keeps it black

        // 5. Load Town
        SceneManager.LoadScene("Town");

    }

    private IEnumerator FadeImage(float from, float to)
    {
        if (fadeImage == null) yield break;

        float t = 0f;
        Color c = fadeImage.color;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            c.a = Mathf.Lerp(from, to, t);
            fadeImage.color = c;
            yield return null;
        }
        c.a = to;
        fadeImage.color = c;
    }

    private IEnumerator FadeText(float from, float to)
    {
        if (cutsceneText == null) yield break;

        float t = 0f;
        Color c = cutsceneText.color;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            c.a = Mathf.Lerp(from, to, t);
            cutsceneText.color = c;
            yield return null;
        }
        c.a = to;
        cutsceneText.color = c;
    }
}
