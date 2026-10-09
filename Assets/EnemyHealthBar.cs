using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    public EnemyHealth enemyHealth;
    public Slider slider;
    public TMP_Text hpText;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    private Transform enemyTransform;

    void Start()
    {
        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth not assigned!");
            enabled = false;
            return;
        }

        if (slider == null)
        {
            Debug.LogError("Slider not assigned!");
            enabled = false;
            return;
        }

        if (hpText == null)
        {
            Debug.LogError("HP Text not assigned!");
            enabled = false;
            return;
        }

        enemyTransform = enemyHealth.transform;

        slider.maxValue = enemyHealth.maxHealth;
        slider.value = enemyHealth.currentHealth;

        UpdateHPText();
    }

    void LateUpdate()
    {
        if (enemyHealth == null) return;

        slider.value = enemyHealth.currentHealth;
        UpdateHPText();

        // 1. POSITIONING: Since the health bar is a child, use localPosition.
        transform.localPosition = offset;

        // 2. COUNTER-FLIPPING THE TEXT: Get the Transform of the text component.
        // Assuming the text object's desired local scale is always (1, 1, 1).
        // This overrides the negative X scale inherited from the enemy, stopping the text from flipping.
        hpText.transform.localScale = new Vector3(
            1f,
            hpText.transform.localScale.y,
            hpText.transform.localScale.z
        );

        // 3. ROTATION: Ensure the main health bar object is not rotated.
        transform.rotation = Quaternion.identity;
    }

    private void UpdateHPText()
    {
        hpText.text = enemyHealth.currentHealth + " / " + enemyHealth.maxHealth;
    }
}
