using System.IO;
using System.Linq;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using System.Collections;


public class AutoSaveManager : MonoBehaviour
{
    public static AutoSaveManager Instance; // Singleton instance

    [Header("Auto Save Settings")]
    public bool enableAutoSave = true;          // Toggle auto-saving
    public float autoSaveInterval = 60f;        // Time in seconds between auto-saves
    public CinemachineConfiner2D currentConfiner; // Reference to current map confiner

    private string saveLocation;                // Path to save file
    private InventoryController inventoryController; // Reference to inventory system
    private HotbarController hotbarController;       // Reference to hotbar system

    private void Awake()
    {
        // Singleton setup: keep only one instance and persist across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        // Subscribe to sceneLoaded event to handle automatic saving/loading when scenes change
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called whenever a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Destroy AutoSaveManager when returning to Main Menu
        if (scene.name == "Main Menu") // Replace with your menu scene name
        {
            Debug.Log("[AutoSaveManager] Main Menu loaded — destroying AutoSaveManager instance.");
            Destroy(gameObject);
            Instance = null;
            return;
        }

        RefreshReferences(); // Ensure references to inventory, hotbar, confiner are up-to-date

        // Optional: perform a save shortly after entering a new scene
        StartCoroutine(SaveAfterSceneLoad());
    }

    private void Start()
    {
        // Define the path to the save file
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        RefreshReferences();

        // If no confiner is assigned, find the first one in the scene
        if (currentConfiner == null)
            currentConfiner = FindFirstObjectByType<CinemachineConfiner2D>();

        // Store last confiner in PlayerPrefs for reference
        if (currentConfiner != null)
        {
            PlayerPrefs.SetString("LastConfiner", currentConfiner.gameObject.name);
            Debug.Log("[AutoSaveManager] Active confiner: " + currentConfiner.gameObject.name);
        }

        // Start auto-saving if enabled
        if (enableAutoSave)
            StartCoroutine(AutoSaveRoutine());
    }

    // Refreshes references to important objects in the scene or DontDestroyOnLoad
    private void RefreshReferences()
    {
        // Find InventoryController in DontDestroyOnLoad scene
        inventoryController = FindObjectsByType<InventoryController>(FindObjectsSortMode.None)
            .FirstOrDefault(ic => ic.gameObject.scene.name == "DontDestroyOnLoad");

        // Find HotbarController in DontDestroyOnLoad scene
        hotbarController = FindObjectsByType<HotbarController>(FindObjectsSortMode.None)
            .FirstOrDefault(hc => hc.gameObject.scene.name == "DontDestroyOnLoad");

        // Find current map confiner in active scene
        currentConfiner = FindFirstObjectByType<CinemachineConfiner2D>();

        Debug.Log($"[AutoSaveManager] References refreshed. " +
                  $"Inventory: {(inventoryController != null ? "Found" : "Missing")}, " +
                  $"Hotbar: {(hotbarController != null ? "Found" : "Missing")}, " +
                  $"Confiner: {(currentConfiner != null ? "Found" : "Missing")}");
    }

    // Coroutine for automatic saving at set intervals
    private IEnumerator AutoSaveRoutine()
    {
        // Wait until player exists in the scene
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player") != null);

        // Loop to save repeatedly as long as auto-save is enabled
        while (enableAutoSave)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            SaveGame();
        }
    }

    // Save shortly after scene loads (optional)
    private IEnumerator SaveAfterSceneLoad()
    {
        yield return new WaitForSeconds(1.5f);
        SaveGame();
    }

    // Save current game state to a JSON file
    public void SaveGame()
    {
        RefreshReferences(); // Ensure all references are current

        var player = GameObject.FindGameObjectWithTag("Player");
        var confiner = FindFirstObjectByType<CinemachineConfiner2D>();

        if (player == null || confiner == null)
        {
            Debug.LogWarning("[AutoSaveManager] Save skipped: Player or Confiner not found.");
            return;
        }

        // Check for null inventory panel to avoid errors
        if (inventoryController != null && inventoryController.inventoryPanel == null)
        {
            Debug.LogWarning("[AutoSaveManager] InventoryController found but inventoryPanel is null. Skipping inventory save.");
        }

        // Create SaveData object storing all necessary info
        SaveData saveData = new SaveData
        {
            playerPosition = player.transform.position,
            mapBoundary = confiner.BoundingShape2D != null ? confiner.BoundingShape2D.gameObject.name : "",
            currentScene = SceneManager.GetActiveScene().name,
            inventorySaveData = (inventoryController != null && inventoryController.inventoryPanel != null)
                ? inventoryController.GetInventoryItems() : new(),
            hotbarSaveData = hotbarController != null ? hotbarController.GetHotbarItems() : new(),
            codexSaveData = CodexManager.Instance != null ? CodexManager.Instance.GetAllUnlockedEntries() : new()
        };

        // Write save data to JSON
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
        Debug.Log($"[AutoSaveManager] Game auto-saved for scene: {SceneManager.GetActiveScene().name}");
    }

    // Load game state from JSON file
    public void LoadGame()
    {
        RefreshReferences();

        if (!File.Exists(saveLocation))
        {
            Debug.Log("[AutoSaveManager] No save file found — creating a new one.");
            SaveGame();
            return;
        }

        // Read JSON and deserialize
        SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Restore player position if still in same scene
        if (player != null && SceneManager.GetActiveScene().name == saveData.currentScene)
        {
            player.transform.position = saveData.playerPosition;
            Debug.Log($"[AutoSaveManager] Restored player position in scene: {saveData.currentScene}");
        }

        // Restore confiner boundaries
        GameObject boundary = GameObject.Find(saveData.mapBoundary);
        var confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        if (confiner != null && boundary != null)
        {
            confiner.BoundingShape2D = boundary.GetComponent<Collider2D>();
            PlayerPrefs.SetString("LastConfiner", boundary.name);
            Debug.Log("[AutoSaveManager] Restored confiner: " + boundary.name);
        }

        // Restore inventory and hotbar items
        inventoryController?.SetInventoryItems(saveData.inventorySaveData);
        hotbarController?.SetHotbarItems(saveData.hotbarSaveData);

        // Restore codex entries
        if (CodexManager.Instance != null && saveData.codexSaveData != null)
        {
            CodexManager.Instance.ResetCodex();
            foreach (string entry in saveData.codexSaveData)
                CodexManager.Instance.UnlockEntry(entry);
        }

        Debug.Log("[AutoSaveManager] Game loaded successfully.");
    }
}
