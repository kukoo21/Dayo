using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI")]
    public TextMeshProUGUI loadingText;
    public GameObject transitionsContainer;

    private SceneTransition[] transitions;

    // 🔔 Event fired when a new scene has fully loaded (after transition)
    public event System.Action<string> OnSceneLoaded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
        if (loadingText != null)
            loadingText.gameObject.SetActive(false);
    }

    public void LoadScene(string sceneName, string transitionName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        SceneTransition transition = transitions.First(t => t.name == transitionName);

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        yield return transition.AnimateTransitionIn();

        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
            loadingText.text = "0%";
        }

        do
        {
            int progressPercent = Mathf.RoundToInt(scene.progress * 100f);
            if (loadingText != null)
                loadingText.text = $"Loading...{progressPercent}%";

            yield return null;
        } while (scene.progress < 0.9f);

        if (loadingText != null)
            loadingText.text = "100%";

        yield return new WaitForSeconds(1f);

        scene.allowSceneActivation = true;

        if (loadingText != null)
            loadingText.gameObject.SetActive(false);

        yield return transition.AnimateTransitionOut();

        // ✅ Notify listeners that the new scene has finished loading
        OnSceneLoaded?.Invoke(sceneName);
        Debug.Log($"[LevelManager] Scene '{sceneName}' finished loading and transitions complete.");
    }
}
