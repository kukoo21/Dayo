using System.Collections.Generic;
using UnityEngine;

public class DialogueTracker : MonoBehaviour
{
    public static DialogueTracker Instance { get; private set; }

    private HashSet<string> playedDialogues = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasPlayed(string dialogueId)
    {
        return playedDialogues.Contains(dialogueId);
    }

    public void MarkAsPlayed(string dialogueId)
    {
        playedDialogues.Add(dialogueId);
    }
}
