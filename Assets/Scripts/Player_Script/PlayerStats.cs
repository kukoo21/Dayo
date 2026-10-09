using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    public float maxMana = 100f;
    public float currentMana;

    public float baseMoveSpeed = 2f;
    public float currentMoveSpeed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ResetStatsToDefault();
    }

    public void ResetStatsToDefault()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentMoveSpeed = baseMoveSpeed;
    }

    public void ModifyStatsForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Town":
                currentMoveSpeed = baseMoveSpeed * 0.8f;
                break;
            case "Battlefield":
                currentMoveSpeed = baseMoveSpeed * 1.2f;
                break;
            default:
                currentMoveSpeed = baseMoveSpeed;
                break;
        }
    }

    // TEMP: Test keys for health/mana changes
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            currentHealth = Mathf.Max(0, currentHealth - 10);

        if (Input.GetKeyDown(KeyCode.J))
            currentHealth = Mathf.Min(maxHealth, currentHealth + 10);

        if (Input.GetKeyDown(KeyCode.M))
            currentMana = Mathf.Max(0, currentMana - 15);

        if (Input.GetKeyDown(KeyCode.N))
            currentMana = Mathf.Min(maxMana, currentMana + 15);
    }
}
