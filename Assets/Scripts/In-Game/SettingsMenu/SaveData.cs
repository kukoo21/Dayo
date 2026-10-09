using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public string currentScene;

    public List<InventorySaveData> inventorySaveData = new();
    public List<InventorySaveData> hotbarSaveData = new();
    public List<string> codexSaveData = new();
}
    