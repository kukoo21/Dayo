using System.Collections.Generic;
using UnityEngine;

public class CodexManager : MonoBehaviour
{
    public static CodexManager Instance;
    private HashSet<string> unlockedEntries = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCodexProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockEntry(string entryID)
    {
        if (string.IsNullOrEmpty(entryID)) return;

        if (!unlockedEntries.Contains(entryID))
        {
            unlockedEntries.Add(entryID);
            Debug.Log("Unlocked Codex Entry: " + entryID);
            SaveCodexProgress();
        }
    }

    public bool IsUnlocked(string entryID)
    {
        return unlockedEntries.Contains(entryID);
    }

    private void SaveCodexProgress()
    {
        string data = string.Join(",", unlockedEntries);
        PlayerPrefs.SetString("CodexEntries", data);
        PlayerPrefs.Save();
    }

    private void LoadCodexProgress()
    {
        string data = PlayerPrefs.GetString("CodexEntries", "");
        unlockedEntries = new HashSet<string>(
            data.Split(',', System.StringSplitOptions.RemoveEmptyEntries)
        );
    }

    public List<string> GetAllUnlockedEntries()
    {
        return new List<string>(unlockedEntries);
    }

    public void ResetCodex()
    {
        unlockedEntries.Clear();
        PlayerPrefs.DeleteKey("CodexEntries");
        PlayerPrefs.Save();
        Debug.Log("[CodexManager] Codex reset");
    }
}
