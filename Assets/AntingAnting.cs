using UnityEngine;
using System.Collections; // Required for Coroutine

public class AntingAnting : MonoBehaviour
{
    [Header("Anting-Anting Settings")]
    public int healthRecoveryAmount = 50; // The amount of HP recovered
    public float cooldownDuration = 5f;   // Cooldown time in seconds
    public KeyCode useKey = KeyCode.V;     // Key to press to use the Anting-Anting

    [Header("UI Reference (Optional)")]
    // If you have a UI Image/Text to show the cooldown, assign it here
    public UnityEngine.UI.Image cooldownImage;
    public TMPro.TMP_Text cooldownText;

    private PlayerHealth playerHealth;
    private bool isOnCooldown = false;
    private float currentCooldownTime = 0f;

    private void Start()
    {
        // Get the PlayerHealth script attached to the same GameObject
        playerHealth = GetComponent<PlayerHealth>();

        // Initialize UI (if references are set)
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f;
            cooldownImage.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // 1. Handle Input
        if (Input.GetKeyDown(useKey) && !isOnCooldown)
        {
            UseAntingAnting();
        }

        // 2. Handle Cooldown
        if (isOnCooldown)
        {
            currentCooldownTime -= Time.deltaTime;

            // Update UI elements while on cooldown
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = currentCooldownTime / cooldownDuration;
                cooldownImage.gameObject.SetActive(true);
            }
            if (cooldownText != null)
            {
                // Display the remaining time rounded up
                cooldownText.text = Mathf.Ceil(currentCooldownTime).ToString();
            }

            if (currentCooldownTime <= 0f)
            {
                isOnCooldown = false;
                currentCooldownTime = 0f;
                FinishCooldownUI();
            }
        }
    }

    public void UseAntingAnting()
    {
        if (playerHealth == null || isOnCooldown) return;

        // Check if the player is already at Max Health
        if (playerHealth.currentHealth >= playerHealth.maxHealth)
        {
            Debug.Log("Anting-Anting not used: Health is already full!");
            // Optionally play an error sound or UI prompt
            return;
        }

        // Apply Health Recovery
        // We use ChangeHealth to ensure UI updates and clamping happen correctly
        playerHealth.ChangeHealth(healthRecoveryAmount);
        Debug.Log($"Used Anting-Anting! Recovered {healthRecoveryAmount} HP.");
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound2D("Anting-anting");
        }
        // Start Cooldown
        StartCooldown();
    }

    private void StartCooldown()
    {
        isOnCooldown = true;
        currentCooldownTime = cooldownDuration;

        // Initial setup for the UI when cooldown starts
        if (cooldownImage != null)
        {
            cooldownImage.gameObject.SetActive(true);
            cooldownImage.fillAmount = 1f;
        }
    }

    private void FinishCooldownUI()
    {
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f;
            cooldownImage.gameObject.SetActive(false);
        }
        if (cooldownText != null)
        {
            cooldownText.text = "";
        }
        // Optionally play a sound/flash to indicate the item is ready
        // SoundManager.Instance?.PlaySound2D("Ready");
    }
}