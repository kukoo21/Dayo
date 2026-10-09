using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.Audio;

public class WelcomeLogo : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI cutsceneText; // assign TMP text
    public Image fadeImage;              // black background image

    [Header("Cutscene Settings")]
    public float textFadeDuration = 3f;
    public float textDisplayTime = 3f;
    public float sceneFadeDuration = 3f;
    public string sceneToLoad = "Main Menu";

    private void Start()
    {

        // Start text invisible
        Color c = cutsceneText.color;
        c.a = 0;
        cutsceneText.color = c;

        // Keep background solid black at start
        fadeImage.color = Color.black;

        // Start coroutine
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

        // Fade whole screen to black (for transition)
        yield return StartCoroutine(FadeScreen(0, 1, sceneFadeDuration));

        // Load main menu scene
        Debug.Log("Loading scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        Color baseColor = cutsceneText.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            cutsceneText.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        cutsceneText.color = new Color(baseColor.r, baseColor.g, baseColor.b, endAlpha);
    }

    private IEnumerator FadeScreen(float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        Color baseColor = fadeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            fadeImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, endAlpha);
    }
}
