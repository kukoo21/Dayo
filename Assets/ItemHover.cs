using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string itemDescription;

    public ItemTooltip tooltip;   // Assign in Inspector
    public Vector3 tooltipOffset = new Vector3(0f, 120f, 0f);

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Show(itemDescription, transform, tooltipOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }
}
