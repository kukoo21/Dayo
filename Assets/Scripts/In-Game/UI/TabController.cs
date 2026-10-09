using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [Header("Tabs")]
    public Image[] tabImages;
    public Button closeBtn;

    [Header("Special Back Buttons")]
    public Button codexBackBtn;      // back button inside Codex
    public Button questLogBackBtn;   // back button inside QuestLog (display page)

    [Header("Pages")]
    public GameObject[] pages;       // normal tab pages

    [Header("Quest Log System")]
    public GameObject questLogPage;  // The parent object for the dedicated display
    public GameObject[] questLogChapterPanels; // Array for the 3 ScrollViews (Ch1, Ch2, Ch3)

    [Header("UI Panel")]
    public GameObject uiPanel;

    private bool isOpen = false;
    private int currentTab = 0;
    private int codexTabIndex = -1;

    void Start()
    {
        uiPanel.SetActive(false);
        ActivateTab(0);

        foreach (var img in tabImages)
            img.color = Color.white;

        closeBtn.onClick.AddListener(CloseUI);
        codexBackBtn.onClick.AddListener(ReturnFromCodex);
        questLogBackBtn.onClick.AddListener(ReturnFromQuestLog);

        // Find codex tab only
        for (int i = 0; i < tabImages.Length; i++)
        {
            if (tabImages[i].name.ToLower().Contains("codex"))
                codexTabIndex = i;
        }

        codexBackBtn.gameObject.SetActive(false);
        questLogBackBtn.gameObject.SetActive(false);

        // Ensure the full quest log page and its sub-panels are hidden on start
        questLogPage.SetActive(false);
        foreach (var panel in questLogChapterPanels) panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // SoundManager.Instance.PlaySound2D("Click"); // Uncomment if sound manager exists
            ToggleUI();
        }
    }

    public void ToggleUI()
    {
        isOpen = !isOpen;
        uiPanel.SetActive(isOpen);

        if (isOpen)
            ActivateTab(0);
    }

    public void ActivateTab(int tabNum)
    {
        // SPECIAL CASE: Codex tab
        if (tabNum == codexTabIndex)
        {
            OpenCodexPage();
            return;
        }

        // NORMAL TAB BEHAVIOR
        HideSpecialPages();

        // Hide all pages first
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            if (i < tabImages.Length)
                tabImages[i].color = Color.grey;
        }

        // Show selected normal page
        if (tabNum < pages.Length)
            pages[tabNum].SetActive(true);

        if (tabNum < tabImages.Length)
            tabImages[tabNum].color = Color.white;

        // Restore normal UI elements
        EnableAllTabs(true);
        closeBtn.gameObject.SetActive(true);

        currentTab = tabNum;
    }

    // -------------------------
    // SPECIAL PAGE: CODEX
    // -------------------------
    private void OpenCodexPage()
    {
        HideAllTabsAndPages();

        if (codexTabIndex != -1 && codexTabIndex < pages.Length)
            pages[codexTabIndex].SetActive(true);

        codexBackBtn.gameObject.SetActive(true);
        closeBtn.gameObject.SetActive(false);
    }

    private void ReturnFromCodex()
    {
        codexBackBtn.gameObject.SetActive(false);
        EnableAllTabs(true);
        closeBtn.gameObject.SetActive(true);
        ActivateTab(0);
    }

    // -------------------------
    // SPECIAL PAGE: QUEST LOG DISPLAY
    // -------------------------

    // Call this function on your Chapter Buttons (Ch1, Ch2, Ch3)
    // Pass 0 for Ch1, 1 for Ch2, 2 for Ch3
    public void OpenQuestLogDisplayPage(int chapterIndex)
    {
        HideAllTabsAndPages();

        questLogPage.SetActive(true);
        questLogBackBtn.gameObject.SetActive(true);
        closeBtn.gameObject.SetActive(false);

        // Hide all internal scroll views first
        foreach (var panel in questLogChapterPanels)
        {
            panel.SetActive(false);
        }

        // Show only the specific chapter requested
        if (chapterIndex >= 0 && chapterIndex < questLogChapterPanels.Length)
        {
            questLogChapterPanels[chapterIndex].SetActive(true);
        }
    }

    public void ReturnFromQuestLog()
    {
        questLogPage.SetActive(false);
        questLogBackBtn.gameObject.SetActive(false);

        EnableAllTabs(true);
        closeBtn.gameObject.SetActive(true);

        ActivateTab(currentTab);
    }

    // -------------------------
    // Helpers
    // -------------------------
    private void HideAllTabsAndPages()
    {
        foreach (var tab in tabImages)
            tab.gameObject.SetActive(false);

        foreach (var page in pages)
            page.SetActive(false);
    }

    private void HideSpecialPages()
    {
        questLogPage.SetActive(false);
        questLogBackBtn.gameObject.SetActive(false);
        codexBackBtn.gameObject.SetActive(false);
    }

    private void EnableAllTabs(bool active)
    {
        foreach (var tab in tabImages)
            tab.gameObject.SetActive(active);
    }

    public void CloseUI()
    {
        isOpen = false;
        uiPanel.SetActive(false);
    }
}