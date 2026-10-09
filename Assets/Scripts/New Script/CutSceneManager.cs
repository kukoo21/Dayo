using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI cutsceneText; // assign TMP text
    public Image fadeImage;              // black background image

    [Header("Cutscene Settings")]
    public float textFadeDuration = 2f;
    public float textDisplayTime = 2f;
    public float sceneFadeDuration = 2f;
    public string sceneToLoad;

    private void Start()
    {

        // Start text invisible
        Color c = cutsceneText.color;
        c.a = 0;
        cutsceneText.color = c;

        // Make sure background is black
        fadeImage.color = Color.black;

        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // Fade text IN
        yield return StartCoroutine(FadeText(0, 1, textFadeDuration));

        // Hold text visible
        yield return new WaitForSeconds(textDisplayTime);

        // Fade text OUT
        yield return StartCoroutine(FadeText(1, 0, textFadeDuration));

        // Fade screen OUT (black overlay in → transition)
        yield return StartCoroutine(FadeScreen(0, 1, sceneFadeDuration));
        if (MusicManager.Instance != null)
        {
        MusicManager.Instance.StopMusic();
        }
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        Color c = cutsceneText.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            cutsceneText.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        cutsceneText.color = new Color(c.r, c.g, c.b, endAlpha);
    }

    private IEnumerator FadeScreen(float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        Color c = fadeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(c.r, c.g, c.b, endAlpha);
    }
}
