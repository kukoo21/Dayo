using UnityEngine;
using TMPro; // Important for TextMeshPro

public class FlashingText : MonoBehaviour
{
    // The rate at which the text will flash (e.g., 5.0 means 5 full cycles per second)
    [Tooltip("The speed of the flashing effect.")]
    public float flashSpeed = 3.0f;

    // The component that holds the text
    private TMP_Text textComponent;

    void Start()
    {
        // Get the TextMeshPro component attached to this GameObject
        textComponent = GetComponent<TMP_Text>();

        if (textComponent == null)
        {
            Debug.LogError("FlashingText script requires a TextMeshPro component on the same GameObject.");
            enabled = false;
        }
    }

    void Update()
    {
        // Calculate the current time's position in a sine wave.
        // The result will oscillate between -1 and 1.
        float sineWave = Mathf.Sin(Time.time * flashSpeed);

        // Map the sine wave from the range [-1, 1] to the alpha range [0, 1].
        // This makes the text fade fully transparent (0) to fully opaque (1).
        float alpha = (sineWave + 1.0f) * 0.5f;

        // Get the current color
        Color currentColor = textComponent.color;

        // Apply the new calculated alpha value
        currentColor.a = alpha;

        // Assign the new color back to the text component
        textComponent.color = currentColor;
    }
}