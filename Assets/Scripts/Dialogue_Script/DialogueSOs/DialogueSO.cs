using UnityEngine;
using System.Collections.Generic; // Added for List<Sprite>

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    public DialogueTextLine[] lines;
    public DialogueOption[] options;

    [Header("Special Settings")]
    public bool isRewardDialogue;
    public string sceneToLoad;

    [Header("Post Dialogue Image Gallery (Optional)")]
    public Sprite[] postDialogueImages; // Array of sprites to show after dialogue

    [Header("Codex Unlock Settings")]
    public bool unlocksCodexEntry;
    public string codexEntryID;

    [Header("Reward Settings")]
    public Sprite rewardIcon;
    public bool givesItemReward;
    public GameObject rewardItemPrefab;

    [Header("Conditional Requirements (Optional)")]
    public ActorSO[] requiredNPCs;
    public LocationSO[] requiredLocations;
    public GameObject[] requiredItems;

    [Header("Quest Settings")]
    public bool unlocksQuest;
    public QuestSO questToUnlock;

    [Header("Quest Kill Requirements (Optional)")]
    public string requiredObjectiveID;
    public int requiredObjectiveAmount = 0;

    public bool isConditionMet()
    {
        // ... (The rest of the isConditionMet logic remains unchanged)

        if (requiredNPCs == null) requiredNPCs = new ActorSO[0];
        if (requiredLocations == null) requiredLocations = new LocationSO[0];
        if (requiredItems == null) requiredItems = new GameObject[0];

        // NPC requirements
        if (requiredNPCs.Length > 0)
        {
            if (DialogueHistoryTracker.Instance == null)
            {
                Debug.LogWarning("[DialogueSO] Missing DialogueHistoryTracker.");
                return false;
            }

            foreach (var npc in requiredNPCs)
            {
                if (npc != null && !DialogueHistoryTracker.Instance.HasSpokenWith(npc))
                    return false;
            }
        }

        // Location requirements
        if (requiredLocations.Length > 0)
        {
            if (LocationHistorytracker.Instance == null)
            {
                Debug.LogWarning("[DialogueSO] Missing LocationHistorytracker.");
                return false;
            }

            foreach (var loc in requiredLocations)
            {
                if (loc != null && !LocationHistorytracker.Instance.HasVisited(loc))
                    return false;
            }
        }

        // Item requirements
        if (requiredItems.Length > 0)
        {
            InventoryController inv = InventoryController.Instance;

            if (inv == null)
            {
                Debug.LogWarning("[DialogueSO] InventoryController.Instance is missing.");
                return false;
            }

            foreach (var itemPrefab in requiredItems)
            {
                if (itemPrefab == null) continue;

                Item item = itemPrefab.GetComponent<Item>();
                if (item == null) continue;

                if (!inv.HasItem(item.ID))
                {
                    Debug.Log($"[DialogueSO] Missing required item: {item.Name}");
                    return false;
                }
            }
        }

        // Quest kill requirement
        if (!string.IsNullOrEmpty(requiredObjectiveID) && requiredObjectiveAmount > 0)
        {
            if (QuestManager.Instance == null)
            {
                Debug.LogWarning("[DialogueSO] Missing QuestManager.");
                return false;
            }

            bool meetsKillRequirement = false;

            foreach (var quest in QuestManager.Instance.activeQuests)
            {
                foreach (var obj in quest.objectives)
                {
                    if (obj.objectiveID == requiredObjectiveID)
                    {
                        if (obj.currentAmount >= requiredObjectiveAmount)
                        {
                            meetsKillRequirement = true;
                            break;
                        }
                    }
                }
                if (meetsKillRequirement) break;
            }

            if (!meetsKillRequirement)
            {
                Debug.Log($"[DialogueSO] Kill requirement NOT met: {requiredObjectiveID}, required {requiredObjectiveAmount}.");
                return false;
            }
        }

        return true;
    }
}

[System.Serializable]
public class DialogueTextLine
{
    public ActorSO speaker;
    [TextArea(3, 5)] public string text;

    // --- NEW: Optional Image for this dialogue line ---
    public Sprite dialogueImage;
    [Tooltip("The size (width, height) of the image display.")]
    public Vector2 imageSize = new Vector2(200, 200);
    [Tooltip("The offset (X, Y) from the image's anchor point.")]
    public Vector2 imageOffset = Vector2.zero;
    // --------------------------------------------------
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueSO nextDialogue;
}