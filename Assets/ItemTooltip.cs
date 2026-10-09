using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    private void Awake()
    {
        tooltipPanel.SetActive(false);
    }

    // Show directly above a specific item
    public void Show(string description, Transform item, Vector3 offset)
    {
        tooltipText.text = description;

        RectTransform canvasRect = tooltipPanel.transform.parent as RectTransform;
        RectTransform itemRect = item as RectTransform;
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

        // UI → UI: use anchoredPosition, not world space
        Vector2 itemScreenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, itemRect.position),
            null,
            out itemScreenPos
        );

        tooltipRect.anchoredPosition = itemScreenPos + (Vector2)offset;

        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
