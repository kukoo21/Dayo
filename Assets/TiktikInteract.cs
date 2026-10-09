using UnityEngine;

public class TiktikInteract : MonoBehaviour, IInteractable
{
    [Header("Dialogue Settings")]
    public DialogueSO dialogue;           // Assign TiktikFoundDialogue
    public bool useRewardUI = true;       // Optional: show dialogue with reward UI style

    [Header("Quest Settings")]
    public string questObjectiveID;       // optional, example: "Find_Tiktik"

    private bool isPlayerInside = false;  // Player in proximity
    private bool hasInteracted = false;   // Prevent multiple interactions

    // Reference to ProximitySoundTrigger (optional)
    private ProximitySoundTrigger soundTrigger;

    private void Awake()
    {
        // Try to get ProximitySoundTrigger if attached
        soundTrigger = GetComponent<ProximitySoundTrigger>();
    }

    // --- IInteractable implementation ---
    public bool CanInteract() => isPlayerInside && !hasInteracted;

    public void Interact()
    {
        if (hasInteracted) return;
        hasInteracted = true;

        // Start dialogue
        if (dialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue, useRewardUI);
        }

        // Update quest progress if needed
        if (!string.IsNullOrEmpty(questObjectiveID) && QuestManager.Instance != null)
        {
            QuestManager.Instance.UpdateQuestProgress(questObjectiveID, 1);
        }

        // Disable sound and notification after interaction
        if (soundTrigger != null)
        {
            soundTrigger.DisableAfterInteraction();
        }
    }

    private void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            isPlayerInside = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            isPlayerInside = false;
    }
}
