using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // REQUIRED for Coroutines

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Initializing default value for clarity
    public int currentHealth;

    [Header("Death UI & Animation")]
    public GameObject deadAnimCanvas; // Main Canvas (must contain the animation image)
    public Animator deadAnimator;
    public string deadAnimTrigger = "Dead"; // The name of the trigger in the Animator

    [Header("Post-Animation Elements")]
    public GameObject gameOverPanel; // Panel containing the text and buttons (MUST be disabled in Inspector)
    [Tooltip("Time (in seconds) to wait after the animation starts before showing the buttons/text.")]
    public float uiDelayAfterAnimStart = 1.0f;

    private void Start()
    {
        currentHealth = maxHealth;

        // Ensure UI is initialized correctly (Only Health UI remains)
        Hud_Controller.Instance.UpdateHealthUI(currentHealth, maxHealth);

        // Ensure the death screen canvas and the game over panel are hidden at the start of the scene
        if (deadAnimCanvas != null)
        {
            deadAnimCanvas.SetActive(false);
        }
        if (gameOverPanel != null)
        {
            // Crucial: The buttons must be hidden initially
            gameOverPanel.SetActive(false);
        }

        // Ensure TimeScale is 1.0 when the player spawns
        Time.timeScale = 1f;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound2D("Hit");
        else
            Debug.LogError("SoundManager.Instance is NULL!");

        if (Hud_Controller.Instance != null)
        {
            Hud_Controller.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogError("Hud_Controller.Instance is NULL at runtime!");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died! Triggering death sequence.");

        // 1. Pause the game
        Time.timeScale = 0f;

        // 2. Show the death screen UI container immediately
        if (deadAnimCanvas != null)
        {
            deadAnimCanvas.SetActive(true);
        }

        // 3. Play the death animation (if animator is assigned)
        if (deadAnimator != null)
        {
            // FIX: Set the animator to update using unscaled time so it ignores Time.timeScale = 0f
            deadAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            deadAnimator.SetTrigger(deadAnimTrigger);
            SoundManager.Instance.PlaySound2D("GameOver");
        }

        // 4. Start the coroutine to wait for the animation to play before showing the buttons
        StartCoroutine(ShowGameOverUI(uiDelayAfterAnimStart));

        // 5. Optionally hide the player's game object or relevant components
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null) playerCollider.enabled = false;

        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        if (playerSprite != null) playerSprite.enabled = false;


        // 6. Disable player controls (replace with your actual script names if different)
        PlayerMovement playerMove = GetComponent<PlayerMovement>();
        if (playerMove != null) playerMove.enabled = false;

        PlayerCombat playerCombat = GetComponent<PlayerCombat>();
        if (playerCombat != null) playerCombat.enabled = false;

        // Disable this script to prevent further damage calls
        this.enabled = false;
    }

    // Coroutine to delay the UI appearance using unscaled time (since timeScale is 0)
    private IEnumerator ShowGameOverUI(float delay)
    {
        // Wait using unscaled time
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime < startTime + delay)
        {
            yield return null; // Wait for the next frame
        }

        // Now show the panel containing the buttons and text
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            SoundManager.Instance?.PlaySound2D("Click"); // Optional UI sound
        }
    }
}