using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Prefab")]
    [Tooltip("Assign your gameplay UI prefab here (e.g., UI_Main).")]
    public GameObject uiPrefab;

    private GameObject uiInstance;

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
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Don’t call InitializeUI here — wait until the first sceneLoaded event
        // SceneManager will automatically invoke OnSceneLoaded for the active scene.
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene loaded: {scene.name}");

        // Scenes where the gameplay UI should NOT be visible
        bool hideUI = scene.name == "Main Menu" || scene.name == "Town" || scene.name == "PlayerHouse" || scene.name == "Cutscene - 1";

        if (hideUI)
        {
            if (uiInstance != null)
            {
                Destroy(uiInstance);
                uiInstance = null;
                Debug.Log($"[GameManager] Destroyed gameplay UI in {scene.name}.");
            }
        }
        else
        {
            // Create the UI only if we’re in a gameplay scene and it doesn’t already exist
            if (uiInstance == null && uiPrefab != null)
            {
                uiInstance = Instantiate(uiPrefab);
                DontDestroyOnLoad(uiInstance);
                Debug.Log($"[GameManager] Created gameplay UI for {scene.name}.");
            }
        }
    }
}
