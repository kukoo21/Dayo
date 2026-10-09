using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IInteractable
{
    public int ID;
    public string Name;
    public int quantity = 1;

    private TMP_Text quantityText;

    [Header("Optional Reward Dialogue")]
    public DialogueSO rewardDialogue;

    private bool hasBeenPickedUp = false;

    private void Awake()
    {
        quantityText = GetComponentInChildren<TMP_Text>();
        UpdateQuantityDisplay();
    }

    public void UpdateQuantityDisplay()
    {
        if (quantityText != null)
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
    }

    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, quantity);
        quantity -= removed;
        UpdateQuantityDisplay();
        return removed;
    }

    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);
        Item cloneItem = clone.GetComponent<Item>();
        cloneItem.quantity = newQuantity;
        cloneItem.UpdateQuantityDisplay();
        return clone;
    }

    // IMPORTANT: Hotbar override point
    public virtual void UseItem()
    {
        Debug.Log("Using base item: " + Name);
    }

    // pickup logic (unchanged)
    public virtual void PickUp()
    {
        if (hasBeenPickedUp) return;
        hasBeenPickedUp = true;

        Sprite itemIcon = GetComponent<Image>()?.sprite;

        if (ItemPickUpUIController.Instance != null)
        {
            ItemPickUpUIController.Instance.ShowItemPickup(Name, itemIcon);
            SoundManager.Instance.PlaySound2D("Blip");
        }

        if (rewardDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(rewardDialogue, useRewardUI: true);
        }

        if (QuestManager.Instance != null)
            QuestManager.Instance.UpdateQuestProgress(ID.ToString(), quantity);

        Destroy(gameObject);
    }

    public bool CanInteract() => !hasBeenPickedUp;

    public void Interact()
    {
        var collector = FindFirstObjectByType<PlayerItemCollector>();
        if (collector != null)
            collector.CollectItem(this);
    }
}
