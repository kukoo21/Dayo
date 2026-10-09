//using UnityEngine;
//using TMPro;

//public class PlayerStatsUI : MonoBehaviour
//{
//    [Header("Bars")]
//    [SerializeField] private BarMaskController hpBar;
//    [SerializeField] private BarMaskController mpBar;

//    [Header("Texts")]
//    [SerializeField] private TMP_Text hpText;
//    [SerializeField] private TMP_Text mpText;

//    private PlayerStats player;

//    void Start()
//    {
//        player = PlayerStats.Instance;

//        hpBar.SetMaxValue(player.maxHealth);
//        mpBar.SetMaxValue(player.maxMana);

//        UpdateUI();
//    }

//    void Update()
//    {
//        hpBar.SetValue(player.currentHealth);
//        mpBar.SetValue(player.currentMana);
//        UpdateUI();
//    }

//    private void UpdateUI()
//    {
//        if (hpText != null)
//            hpText.text = $"{Mathf.RoundToInt(player.currentHealth)}/{Mathf.RoundToInt(player.maxHealth)}";

//        if (mpText != null)
//            mpText.text = $"{Mathf.RoundToInt(player.currentMana)}/{Mathf.RoundToInt(player.maxMana)}";
//    }
//}
