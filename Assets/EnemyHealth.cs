using UnityEngine;
using System.Collections;
using System; // REQUIRED: For the Action event subscription (OnDialogueEnded)

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Health Bar")]
    public GameObject healthBarUI;

    [Header("Damage Flash Effect")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.05f;
    public int numberOfFlashes = 4;

    [Header("Death Settings")]
    public float deathAnimationDuration = 0.8f;

    [Header("Quest Settings")]
    public string defeatObjectiveID = "Defeat_Amaranhig";

    [Header("Optional Dialogue On Death")]
    public bool playDialogueOnDeath = false;     // ✅ TOGGLE
    public DialogueSO dialogueAfterDeath;        // ✅ DIALOGUE TO PLAY
    public float dialogueDelay = 0.2f;           // ✅ OPTIONAL DELAY
    public bool reenablePlayerMovement = true;   // NEW: Controls whether player movement is re-enabled

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Animator animator;
    private Enemy_movement enemyMovement;
    private PlayerMovement playerMovement; // NEW: Reference to the PlayerMovement script
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBarUI != null)
            healthBarUI.SetActive(false);

        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        enemyMovement = GetComponent<Enemy_movement>();

        // NEW: Find the PlayerMovement script
        // Assumes PlayerMovement is on the player GameObject in the scene
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound2D("Hit");
        }
        Debug.Log("Enemy hit! HP left: " + currentHealth);

        StartFlash();

        if (healthBarUI != null && !healthBarUI.activeSelf)
        {
            healthBarUI.SetActive(true);
            HideNotifyIcon();
        }

        if (currentHealth <= 0)
            Die();
    }

    public void StartFlash()
    {
        if (spriteRenderer == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashEffectCoroutine());
    }

    private IEnumerator FlashEffectCoroutine()
    {
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        spriteRenderer.color = originalColor;
    }

    public void HideNotifyIcon()
    {
        if (enemyMovement != null && enemyMovement.notifyIcon != null)
            enemyMovement.notifyIcon.SetActive(false);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // ✅ Update Quest Progress
        if (!string.IsNullOrEmpty(defeatObjectiveID))
            QuestManager.Instance?.UpdateQuestProgress(defeatObjectiveID, 1);

        // ✅ Play death animation
        if (animator != null)
            animator.SetTrigger("dead");

        // ✅ Disable physics & UI
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        if (healthBarUI != null)
            healthBarUI.SetActive(false);

        HideNotifyIcon();

        if (enemyMovement != null)
            enemyMovement.enabled = false;

        // ✅ OPTIONAL: Play dialogue if enabled
        if (playDialogueOnDeath && dialogueAfterDeath != null)
        {
            // NEW: Disable player movement right before starting dialogue
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                Debug.Log("Player controls disabled for post-death dialogue.");
            }

            // NEW: Subscribe to the dialogue end event
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueEnded += ReenableControls;
            }

            StartCoroutine(PlayDialogueAfterDelay());
        }

        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator PlayDialogueAfterDelay()
    {
        yield return new WaitForSeconds(dialogueDelay);
        DialogueManager.Instance?.StartDialogue(dialogueAfterDeath);
    }

    // NEW: Function called when DialogueManager signals the dialogue is over
    private void ReenableControls()
    {
        // Unsubscribe immediately to prevent calling this multiple times
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded -= ReenableControls;
        }

        if (reenablePlayerMovement && playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("Player controls re-enabled after enemy dialogue.");
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);

        // Destroy the enemy GameObject after the death animation plays
        Destroy(gameObject);
    }
}