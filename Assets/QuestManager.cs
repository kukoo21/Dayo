using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<QuestProgress> activeQuests = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateAllUI()
    {
        QuestUIController.Instance?.UpdateQuestUI();
    }

    public void AddQuest(QuestSO quest)
    {
        // Prevent duplicates
        foreach (var q in activeQuests)
            if (q.quest.questID == quest.questID)
                return;

        // ONLY HERE remove completed quests
        RemoveCompletedQuests();

        // Add the new quest
        QuestProgress newQuest = new QuestProgress(quest);
        activeQuests.Add(newQuest);

        QuestUIController.Instance?.UpdateQuestUI();
    }

    public void UpdateQuestProgress(string objectiveID, int amount = 1)
    {
        foreach (var quest in activeQuests)
        {
            foreach (var obj in quest.objectives)
            {
                if (obj.objectiveID == objectiveID && obj.type != ObjectiveType.RequiredLocation)
                {
                    obj.currentAmount += amount;

                    if (obj.currentAmount > obj.requiredAmount)
                        obj.currentAmount = obj.requiredAmount;
                }
            }
        }

        // Do NOT remove quests here
        QuestUIController.Instance?.UpdateQuestUI();
    }

    public void CompleteLocationObjective(string locationID)
    {
        foreach (var quest in activeQuests)
        {
            foreach (var obj in quest.objectives)
            {
                if (obj.type == ObjectiveType.RequiredLocation && obj.objectiveID == locationID)
                {
                    obj.MarkComplete();
                }
            }
        }

        // Do NOT remove quests here
        QuestUIController.Instance?.UpdateQuestUI();
    }

    private void RemoveCompletedQuests()
    {
        activeQuests.RemoveAll(q => q.IsCompleted);
    }

    public void ResetAllQuests()
    {
        activeQuests.Clear();
        QuestUIController.Instance?.UpdateQuestUI();
    }
}
