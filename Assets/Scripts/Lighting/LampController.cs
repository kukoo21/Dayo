using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampController : MonoBehaviour
{
    [Header("Light and Cycle References")]
    public Light2D lampLight;
    public DayNightCycle cycle;

    [Header("Sprites")]
    public SpriteRenderer lampRenderer;  // SpriteRenderer for the lantern
    public Sprite daySprite;             // Lantern off sprite
    public Sprite nightSprite;           // Lantern on sprite

    [Header("Fade Settings")]
    public float fadeSpeed = 2f;         // How fast the fade happens
    public float maxLightIntensity = 1.5f; // Brightness when fully on

    private float targetIntensity;

    void Start()
    {
        // Initialize
        lampLight.intensity = 0f;
        lampLight.enabled = true; // Always on, we’ll control visibility via intensity
        lampRenderer.sprite = daySprite;
    }

    void Update()
    {
        // Choose target intensity based on time of day
        targetIntensity = cycle.isDay ? 0f : maxLightIntensity;

        // Smoothly fade the light in or out
        lampLight.intensity = Mathf.Lerp(lampLight.intensity, targetIntensity, Time.deltaTime * fadeSpeed);

        // Switch lantern sprite depending on state
        lampRenderer.sprite = cycle.isDay ? daySprite : nightSprite;

        // Keep sprite fully opaque (no transparency)
        lampRenderer.color = Color.white;
    }
}
