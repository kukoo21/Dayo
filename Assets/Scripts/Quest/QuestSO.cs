using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "QuestSO")]
public class QuestSO : ScriptableObject
{
    public string questID;
    public string questName;
    public string questChapter;
    public string questDescription;
    public List<QuestObjective> objectives;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(questID))
            questID = questName + "_" + Guid.NewGuid().ToString();
    }
}

[Serializable]
public class QuestObjective
{
    public string objectiveID;
    public string description;
    public ObjectiveType type;
    public int requiredAmount = 1;
    public int currentAmount;

    public bool IsCompleted => currentAmount >= requiredAmount;

    public void MarkComplete()
    {
        currentAmount = requiredAmount;
    }
}

public enum ObjectiveType
{
    CollectItem,
    DefeatEnemy,
    Survive,
    WinRace,
    FindEnemy,
    WinRiddle,
    RequiredLocation
}

[Serializable]
public class QuestProgress
{
    public QuestSO quest;
    public List<QuestObjective> objectives;

    public QuestProgress(QuestSO quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjective>();

        foreach (var obj in quest.objectives)
        {
            objectives.Add(new QuestObjective
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            });
        }
    }

    public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted);
    public string QuestID => quest.questID;
}
