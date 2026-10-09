using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro; // for TextMeshPro

public class DayNightCycle : MonoBehaviour
{
    [Header("Lighting Settings")]
    public Light2D globalLight;
    public float interval = 10f; // 10 seconds per phase

    [Header("UI Settings")]
    public TMP_Text clockText; // Assign your TMP Text here

    private float timer = 0f;
    private float totalTime = 0f; // Keeps counting forever
    [HideInInspector] public bool isDay = true;

    void Start()
    {
        isDay = true;
        timer = 0f;
        totalTime = 0f;
        globalLight.intensity = 1f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        totalTime += Time.deltaTime;

        // Switch between day and night every interval
        if (timer >= interval)
        {
            isDay = !isDay;
            timer = 0f;
        }

        // Smoothly change light intensity
        float targetIntensity = isDay ? 1f : 0.1f;
        globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetIntensity, Time.deltaTime * 2f);

        // Update the UI clock
        UpdateClockText();
    }

    void UpdateClockText()
    {
        // Convert total time into minutes and seconds
        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime % 60f);

        //string phase = isDay ? "Day" : "Night";
        clockText.text = $"Time: {minutes:00}:{seconds:00}";
    }
}
