using UnityEngine;
using UnityEngine.SceneManagement; // Required for SceneManager

public class Settings : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsMenu;      // The main pause/settings menu
    public GameObject audioSettings;     // The audio settings submenu

    private bool isPaused = false;

    private void Update()
    {
        // Pause or unpause with ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Only allow pausing if the death canvas is not active
            if (GetComponent<Canvas>() != null && !GetComponent<Canvas>().enabled)
            {
                TogglePause();
                SoundManager.Instance?.PlaySound2D("Click");
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        // Pause or resume game
        Time.timeScale = isPaused ? 0f : 1f;

        // Show or hide main settings
        if (settingsMenu != null)
            settingsMenu.SetActive(isPaused);

        // Hide audio settings when resuming
        if (audioSettings != null && !isPaused)
            audioSettings.SetActive(false);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (settingsMenu != null)
            settingsMenu.SetActive(false);
    }

    public void OpenAudioSettings()
    {
        if (settingsMenu != null)
            settingsMenu.SetActive(false);

        if (audioSettings != null)
            audioSettings.SetActive(true);
    }

    public void CloseAudioSettings()
    {
        if (audioSettings != null)
            audioSettings.SetActive(false);

        if (settingsMenu != null)
            settingsMenu.SetActive(true);
    }

    // NEW FUNCTION: Restarts the current scene
    public void RestartScene()
    {
        SoundManager.Instance?.PlaySound2D("Click");

        // Ensure time is running before scene change
        Time.timeScale = 1f;

        // Reset all quests
        if (QuestManager.Instance != null)
            QuestManager.Instance.ResetAllQuests();

        // Get the name of the currently active scene
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Optionally, use your LevelManager for transitions if it supports restarting by name
        if (LevelManager.Instance != null)
            LevelManager.Instance.LoadScene(currentSceneName, "CrossFade");
        else
            SceneManager.LoadScene(currentSceneName);
    }

    public void Quit()
    {
        SoundManager.Instance?.PlaySound2D("Click");

        // Restore time before quitting
        Time.timeScale = 1f;

        // Hide the settings UI before transitioning
        if (settingsMenu != null)
            settingsMenu.SetActive(false);
        if (audioSettings != null)
            audioSettings.SetActive(false);

        // Optionally disable the entire canvas if your settings are under one
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            canvas.enabled = false;

        // Load main menu
        if (LevelManager.Instance != null)
            LevelManager.Instance.LoadScene("Main Menu", "CrossFade");

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic("MainMenu");
    }

}