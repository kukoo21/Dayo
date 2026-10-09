using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hud_Controller : MonoBehaviour
{
    public static Hud_Controller Instance;

    [Header("Health UI")]
    public Slider healthSlider;
    public TMP_Text healthText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void UpdateHealthUI(int current, int max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;

        if (healthText != null)
            healthText.text = current + " / " + max;
    }
}
