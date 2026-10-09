using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUIController : MonoBehaviour
{
    public static QuestUIController Instance;

    public RectTransform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;
    public GameObject chapterTextPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.UpdateAllUI();
    }

    public void UpdateQuestUI()
    {
        // 1. Destroy existing UI entries
        foreach (Transform child in questListContent)
            Destroy(child.gameObject);

        // 2. Build the new UI entries
        foreach (var quest in QuestManager.Instance.activeQuests)
        {
            // 3. Instantiate the Quest Entry (Parented to Content)
            GameObject entry = Instantiate(questEntryPrefab, questListContent);

            // 4. Instantiate the Chapter Header *inside* the Quest Entry

            // FIX: Use the overload (original, parent, worldPositionStays) to ensure parenting.
            // worldPositionStays: false ensures the new object adopts the parent's scale/rotation.
            GameObject chapterHeader = Instantiate(chapterTextPrefab, entry.transform, false);

            // FIX: Set the sibling index to 0. This forces the chapter text to the very top 
            // of the QuestEntry's Vertical Layout Group.
            chapterHeader.transform.SetSiblingIndex(0);

            // Set the chapter text
            TMP_Text chapterText = chapterHeader.GetComponentInChildren<TMP_Text>();
            if (chapterText != null)
            {
                chapterText.text = $"[ {quest.quest.questChapter} ]";
            }

            // 5. Find and set the Quest Name text 
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();

            // Find the Objectives list container
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            questNameText.text = quest.quest.questName;

            // 6. Instantiate Objectives
            foreach (var obj in quest.objectives)
            {
                GameObject line = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text text = line.GetComponent<TMP_Text>();

                text.text = $"{obj.description} ({obj.currentAmount}/{obj.requiredAmount})";
            }
        }

        // 7. START THE LAYOUT REFRESH (Still necessary for nested layouts)
        StartCoroutine(RefreshLayout(questListContent));
    }

    /// <summary>
    /// Forces the layout to rebuild one frame later to account for nested Content Size Fitters.
    /// This fixes the UI cluttering issue on game start.
    /// </summary>
    private IEnumerator RefreshLayout(RectTransform layoutRoot)
    {
        // Wait one frame for the text components to finalize their preferred sizes.
        yield return null;

        // Force the root Layout Group (on questListContent) to recalculate.
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
    }
}