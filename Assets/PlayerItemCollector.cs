using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;

    void Start()
    {
        FindInventoryController();
    }


    private void FindInventoryController()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
        if (inventoryController == null)
        {
            Debug.LogWarning("InventoryController not found in scene.");
        }
    }

    // This remains, but no longer does auto-pickup
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("QuestItem"))
        {
            // Just detect the item, don't pick it up yet
            Debug.Log($"[Collector] Detected quest item: {collision.name}");
        }
    }

    // This helper will be called from Item.Interact()
    public void CollectItem(Item item)
    {
        if (item == null || inventoryController == null) return;

        bool itemAdded = inventoryController.AddItem(item.gameObject);
        if (itemAdded)
        {
            item.PickUp();
            Destroy(item.gameObject);
        }
    }
}
