using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Required for loading the next scene

public class CanvasFaderManager : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("The time it takes for one Canvas to fully fade out/in (in seconds).")]
    public float fadeDuration = 0.5f;

    [Tooltip("How long each canvas remains fully visible before transitioning to the next (in seconds).")]
    public float displayTime = 3f;

    [Header("Next Scene")]
    [Tooltip("The name of the scene to load after the end credits sequence is complete.")]
    public string nextSceneName = "MainMenu";

    [Header("Managed Canvases")]
    [Tooltip("Drag the Canvas Group component from each of your four Canvases here. They will play in the order listed.")]
    public List<CanvasGroup> allCanvasGroups = new List<CanvasGroup>();

    void Start()
    {
        // 1. Set all canvases to the correct initial state for fading (invisible).
        SetupInitialCanvasStates();

        // 2. Start the automated end credits sequence immediately.
        StartCoroutine(PlayEndCreditsSequence());
    }

    /// <summary>
    /// Ensures all canvases start invisible, non-interactive, and inactive.
    /// </summary>
    private void SetupInitialCanvasStates()
    {
        foreach (CanvasGroup group in allCanvasGroups)
        {
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            // Ensure the game object is off until its turn to fade in
            group.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Manages the sequential fade-in/display/fade-out of all credit screens.
    /// </summary>
    private IEnumerator PlayEndCreditsSequence()
    {
        // Ensure there are canvases to show
        if (allCanvasGroups.Count == 0)
        {
            Debug.LogError("No Canvas Groups assigned to the 'allCanvasGroups' list!");
            yield break; // Stop the coroutine
        }

        // Iterate through every canvas in the list
        for (int i = 0; i < allCanvasGroups.Count; i++)
        {
            CanvasGroup currentCanvas = allCanvasGroups[i];

            // --- PHASE 1: FADE IN THE CURRENT CANVAS ---
            currentCanvas.gameObject.SetActive(true);
            currentCanvas.blocksRaycasts = true;
            currentCanvas.interactable = true;

            // Perform the fade-in animation
            yield return StartCoroutine(Fade(currentCanvas, 1f));

            // --- PHASE 2: DISPLAY TIME ---
            // Wait for the defined duration while the canvas is fully visible
            yield return new WaitForSeconds(displayTime);

            // --- PHASE 3: FADE OUT THE CURRENT CANVAS (unless it's the last one) ---
            if (i < allCanvasGroups.Count - 1)
            {
                // Perform the fade-out animation
                yield return StartCoroutine(Fade(currentCanvas, 0f));

                // Deactivate the old canvas after it's fully invisible
                currentCanvas.blocksRaycasts = false;
                currentCanvas.interactable = false;
                currentCanvas.gameObject.SetActive(false);
            }
            else
            {
                // It's the final canvas, simply fade out and end the sequence.
                yield return StartCoroutine(Fade(currentCanvas, 0f));
                currentCanvas.blocksRaycasts = false;
                currentCanvas.interactable = false;
                currentCanvas.gameObject.SetActive(false);

                // --- FINAL STEP: LOAD NEXT SCENE ---
                Debug.Log("End Credits sequence complete! Loading next scene: " + nextSceneName);
                LevelManager.Instance.LoadScene(nextSceneName,"CrossFade");
                if(SoundManager.Instance != null) 
                {
                    SoundManager.Instance.PlaySound2D("MainMenu");
                }
            }
        }
    }

    /// <summary>
    /// Generic coroutine to smoothly transition the CanvasGroup alpha.
    /// </summary>
    /// <param name="targetGroup">The Canvas Group component to fade.</param>
    /// <param name="targetAlpha">The final alpha value (0 for out, 1 for in).</param>
    private IEnumerator Fade(CanvasGroup targetGroup, float targetAlpha)
    {
        float startAlpha = targetGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            // Use smooth step for better visual appearance (optional, but nice)
            float t = time / fadeDuration;
            targetGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        // Ensure the alpha is exactly the target value upon completion
        targetGroup.alpha = targetAlpha;
    }
}