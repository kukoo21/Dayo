using System.Collections.Generic;
using UnityEngine;

public class DialogueHistoryTracker : MonoBehaviour
{
    public static DialogueHistoryTracker Instance;
    private readonly List<ActorSO> spokenNPCs = new List<ActorSO>();

    private void Start()
    {
        Instance = this;
    }

    public void RecordNPC(ActorSO actorSO)
    {
        if (actorSO == null) return;
        if (!spokenNPCs.Contains(actorSO))
        {
            spokenNPCs.Add(actorSO);
            Debug.Log("Recorded NPC: " + actorSO.actorName);
        }
    }

    public bool HasSpokenWith(ActorSO actorSO)
    {
        return spokenNPCs.Contains(actorSO);
    }
}
