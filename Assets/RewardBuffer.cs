using System.Collections.Generic;
using UnityEngine;

public class RewardBuffer : MonoBehaviour
{
    public static RewardBuffer Instance;
    public List<GameObject> pendingRewards = new();

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

    public void AddReward(GameObject prefab)
    {
        pendingRewards.Add(prefab);
    }

    public void ApplyRewardsToInventory()
    {
        var inv = FindFirstObjectByType<InventoryController>();
        if (inv == null) return;

        foreach (var prefab in pendingRewards)
        {
            inv.AddItem(prefab);
        }

        pendingRewards.Clear();
    }
}
