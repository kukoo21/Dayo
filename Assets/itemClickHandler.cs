using UnityEngine;
using UnityEngine.EventSystems;

public class ItemClickHandler : MonoBehaviour, IPointerClickHandler
{
    private PlayerCombat playerCombat;
    private Item item; // optional: if item has type info

    void Start()
    {
        playerCombat = FindFirstObjectByType<PlayerCombat>();
        item = GetComponent<Item>(); // if you want item types
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Debug.Log("Clicked item: " + gameObject.name);

        // EXAMPLE: Call attack in PlayerCombat
        if (playerCombat != null)
        {
            playerCombat.Attack(playerCombat.lastDirection);
        }
        else
        {
            Debug.LogWarning("PlayerCombat not found!");
        }
    }
}
