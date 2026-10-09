using UnityEngine;
using UnityEngine.UI;

public class BarMaskController : MonoBehaviour
{
    //[SerializeField] private RectMask2D rectMask;
    //[SerializeField] private RectTransform barFill; // The image inside the mask
    //[SerializeField] private float maxRightPadding = 0f; // full bar
    //[SerializeField] private float minRightPadding = 200f; // empty bar

    //private float maxValue = 100f;
    //private float currentValue = 100f;

    //private void Start()
    //{
    //    if (rectMask == null)
    //        rectMask = GetComponent<RectMask2D>();
    //}

    //public void SetMaxValue(float value)
    //{
    //    maxValue = value;
    //    currentValue = value;
    //    UpdateBar();
    //}

    //public void SetValue(float value)
    //{
    //    currentValue = Mathf.Clamp(value, 0, maxValue);
    //    UpdateBar();
    //}

    //private void UpdateBar()
    //{
    //    if (rectMask == null) return;

    //    var padding = rectMask.padding;
    //    float fillPercent = currentValue / maxValue;
    //    padding.right = Mathf.Lerp(minRightPadding, maxRightPadding, fillPercent);
    //    rectMask.padding = padding;
    //}
}
