using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class MainMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("New Game Button")]
    public Button newGameButton;
    public TMP_Text continueLabel;

    [Header("Continue Button")]
    public Button continueButton;

    // ⬇ CRITICAL: Reference to the CanvasGroup on your button container
    [Header("Main Menu UI Container")]
    [Tooltip("The Canvas Group component attached to the parent of your main menu buttons (e.g., 'MainMenuButtonsGroup').")]
    public CanvasGroup mainMenuCanvasGroup;

    [Header("Confirmation Pop-up")]
    [Tooltip("The root panel GameObject of the confirmation pop-up.")]
    public GameObject confirmationPanel;
    [Tooltip("The text field showing details (e.g., 'Warning: Saved game found!').")]
    public TMP_Text popUpMessageText;
    [Tooltip("Button to confirm deleting save and starting a new game.")]
    public Button confirmNewButton;
    [Tooltip("Button to cancel and return to main menu.")]
    public Button cancelButton;

    private string saveLocation;
    private string backupSaveLocation;
    private const string NEW_GAME_SCENE = "Cutscene - 1";

    private void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        backupSaveLocation = Path.Combine(Application.persistentDataPath, "backup_saveData.json");

        LoadVolume();

        if (continueLabel != null)
            continueLabel.gameObject.SetActive(false);

        // Hide the pop-up panel on start
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        // Ensure main menu is fully displayed and interactable on start
        SetMainMenuDisplay(true);

        // Continue Button is only interactable if a save exists
        if (continueButton != null)
            continueButton.interactable = File.Exists(saveLocation);
    }

    // -----------------------------------------------------------------------
    // ⬇ MODIFIED: Method to control the main menu's visibility AND interactivity
    // -----------------------------------------------------------------------
    private void SetMainMenuDisplay(bool isVisible)
    {
        if (mainMenuCanvasGroup != null)
        {
            // If visible: Full alpha, interactable, blocks raycasts
            mainMenuCanvasGroup.alpha = isVisible ? 1f : 0f;
            mainMenuCanvasGroup.interactable = isVisible;
            mainMenuCanvasGroup.blocksRaycasts = isVisible;
        }
        else
        {
            // Fallback: This is less robust but provides some control
            if (newGameButton != null) newGameButton.gameObject.SetActive(isVisible);
            // We only show continue if a file exists, but we can hide it unconditionally.
            if (continueButton != null) continueButton.gameObject.SetActive(isVisible);
        }
    }


    // -----------------------------------------------------------------------
    // ⬇ MODIFIED: Play() now disables the main menu and shows the prompt.
    // -----------------------------------------------------------------------
    public void Play()
    {
        if (File.Exists(saveLocation))
        {
            if (confirmationPanel == null || popUpMessageText == null)
            {
                Debug.LogError("Confirmation UI not assigned. Starting new game anyway.");
                PlayNewGame();
                return;
            }

            // 1. DISABLE the main menu UI completely
            SetMainMenuDisplay(false);

            // 2. Configure and show the pop-up
            popUpMessageText.text = "Saved data found.\nPlaying a new game will overide the data.\nAre your sure you want to continue?";

            if (confirmNewButton != null)
            {
                confirmNewButton.onClick.RemoveAllListeners();
                confirmNewButton.onClick.AddListener(ConfirmNewGame);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(CancelAction);
            }

            confirmationPanel.SetActive(true);
        }
        else
        {
            PlayNewGame();
        }
    }

    public void PlayNewGame()
    {
        // Ensure the menu is fully displayed before leaving, just in case
        SetMainMenuDisplay(true);
        DeleteSaveFiles();
        CodexManager.Instance?.ResetCodex();
        LevelManager.Instance.LoadScene(NEW_GAME_SCENE, "CrossFade");
        MusicManager.Instance.StopMusic();
    }

    // ⬇ MODIFIED: Confirmation handler re-enables the main menu.
    public void ConfirmNewGame()
    {
        CancelAction(); // Hides the prompt and runs cleanup logic
        PlayNewGame();  // Deletes files and starts game
    }

    // ⬇ MODIFIED: Cancel handler re-enables the main menu.
    public void CancelAction()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        // RE-ENABLE the main menu UI
        SetMainMenuDisplay(true);
    }

    // -----------------------------------------------------------------------
    // ... (ContinueGame, LoadAndRestoreGame, DeleteSaveFiles, Quit, and Volume methods remain unchanged)
    // -----------------------------------------------------------------------

    public void ContinueGame()
    {
        if (!File.Exists(saveLocation))
        {
            Debug.LogWarning("No save file found — cannot continue.");
            return;
        }

        string json = File.ReadAllText(saveLocation);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        if (string.IsNullOrEmpty(saveData.currentScene))
        {
            Debug.LogWarning("Save file has no valid scene name. Starting new game instead.");
            PlayNewGame();
            return;
        }

        StartCoroutine(LoadAndRestoreGame(saveData.currentScene));
    }

    private IEnumerator LoadAndRestoreGame(string sceneName)
    {
        bool sceneReady = false;

        LevelManager.Instance.OnSceneLoaded += (loadedScene) =>
        {
            if (loadedScene == sceneName)
                sceneReady = true;
        };

        LevelManager.Instance.LoadScene(sceneName, "CrossFade");
        MusicManager.Instance?.StopMusic();

        yield return new WaitUntil(() => sceneReady);

        yield return new WaitForSeconds(0.5f);

        var autoSaveManager = FindFirstObjectByType<AutoSaveManager>();
        if (autoSaveManager != null)
        {
            autoSaveManager.LoadGame();
            Debug.Log($"[MainMenu] Continue Game: Loaded save data after scene '{sceneName}' finished loading.");
        }
        else
        {
            Debug.LogWarning("[MainMenu] AutoSaveManager not found after scene load.");
        }
    }

    private void DeleteSaveFiles()
    {
        if (File.Exists(saveLocation))
            File.Delete(saveLocation);

        if (File.Exists(backupSaveLocation))
            File.Delete(backupSaveLocation);

        PlayerPrefs.DeleteKey("LastConfiner");
        PlayerPrefs.Save();

        Debug.Log("Deleted all save data.");
    }

    public void Quit() => Application.Quit();

    // Volume methods
    public void UpdateMusicVolume(float volume) => audioMixer.SetFloat("MusicVolume", volume);
    public void UpdateSoundVolume(float volume) => audioMixer.SetFloat("SFXVolume", volume);

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float mv);
        PlayerPrefs.SetFloat("MusicVolume", mv);
        audioMixer.GetFloat("SFXVolume", out float sv);
        PlayerPrefs.SetFloat("SFXVolume", sv);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
}