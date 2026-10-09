using UnityEngine;

public class DialogueActivator : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueId;
    [SerializeField] private Dialogue dialogue;

    [Header("Quest Unlock Settings")]
    public bool unlocksQuest = false;
    public QuestSO questToUnlock;

    [Header("Activation Mode")]
    public bool autoPlayOnStart = false; // plays once when scene starts
    public bool triggerOnEnter = false; // plays when player enters trigger

    [Header("Disable Condition")]
    [Tooltip("If assigned, dialogue will disable once player has spoken with this NPC.")]
    public ActorSO requiredActor; // optional NPC requirement

    [Header("Load Scene After Dialogue")]
    public string sceneToLoad;

    private bool alreadyTriggered = false;
    private Collider2D triggerCollider;

    private bool hasPlayed =>
        DialogueTracker.Instance != null &&
        DialogueTracker.Instance.HasPlayed(dialogueId);

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        UpdateDialogueState();

        if (autoPlayOnStart && !hasPlayed && IsEnabled())
        {
            PlayDialogue();
        }
    }

    private void Update()
    {
        UpdateDialogueState();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerOnEnter &&
            !hasPlayed &&
            !alreadyTriggered &&
            IsEnabled() &&
            collision.CompareTag("Player"))
        {
            alreadyTriggered = true;
            PlayDialogue();
        }
    }

    private void PlayDialogue()
    {
        if (CutsceneDialogueManager.Instance != null)
        {
            // ⬇ NEW: Register a callback when dialogue ends
            CutsceneDialogueManager.Instance.onDialogueFinished += OnDialogueFinished;

            CutsceneDialogueManager.Instance.StartDialogue(dialogue);
        }

        if (DialogueTracker.Instance != null)
            DialogueTracker.Instance.MarkAsPlayed(dialogueId);
    }

    // ⬇ NEW CALLBACK — Runs AFTER the dialogue fully ends
    private void OnDialogueFinished()
    {
        // Avoid double events
        CutsceneDialogueManager.Instance.onDialogueFinished -= OnDialogueFinished;

        // UNLOCK QUEST (NEW)
        if (unlocksQuest && questToUnlock != null)
        {
            QuestManager.Instance.AddQuest(questToUnlock);
            Debug.Log("[DialogueActivator] Unlocked Quest: " + questToUnlock.questName);
        }

        // LOAD SCENE (optional)
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            LevelManager.Instance.LoadScene(sceneToLoad, "CrossFade");
        }

        // OPTIONAL: Disable after unlocking
        if (unlocksQuest)
        {
            gameObject.SetActive(false);
        }
    }

    private void UpdateDialogueState()
    {
        if (requiredActor == null || DialogueHistoryTracker.Instance == null)
            return;

        bool hasSpoken = DialogueHistoryTracker.Instance.HasSpokenWith(requiredActor);

        if (triggerCollider != null)
            triggerCollider.enabled = !hasSpoken;
    }

    private bool IsEnabled()
    {
        if (requiredActor == null || DialogueHistoryTracker.Instance == null)
            return true;

        return !DialogueHistoryTracker.Instance.HasSpokenWith(requiredActor);
    }
}
