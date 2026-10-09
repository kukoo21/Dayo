using UnityEngine;
using UnityEngine.UI;

public class QuestLogController : MonoBehaviour
{
    [Header("Quest Log Settings")]
    public GameObject questLogPage;         // Main ScrollView Quest List
    public GameObject[] questButtons;       // Quest list buttons

    [Header("Quest Detail Pages")]
    public GameObject[] questPages;

    [Header("Other UI Panels To Hide")]
    public GameObject[] otherUIPanels;      // UI panels to disable while quest log is open

    void Start()
    {
        InitializeQuestLog();
    }

    private void InitializeQuestLog()
    {
        // Assign button listeners
        for (int i = 0; i < questButtons.Length; i++)
        {
            int index = i;
            Button btn = questButtons[i].GetComponent<Button>();
            btn.onClick.AddListener(() => OnQuestButtonClicked(index));
        }

        // Hide all quest detail pages
        foreach (var page in questPages)
            page.SetActive(false);

        // Show the quest log
        questLogPage.SetActive(true);

        // Disable all other UI
        SetOtherUIPanelsActive(false);
    }

    private void OnQuestButtonClicked(int index)
    {
        questLogPage.SetActive(false);

        foreach (var page in questPages)
            page.SetActive(false);

        if (index < questPages.Length)
            questPages[index].SetActive(true);

        if (SoundManager.Instance)
            SoundManager.Instance.PlaySound2D("Click");
    }

    public void ReturnToQuestLog()
    {
        // Hide all quest detail pages
        foreach (var page in questPages)
            page.SetActive(false);

        // Reactivate other UI panels
        SetOtherUIPanelsActive(true);

        // Hide the quest log entirely
        questLogPage.SetActive(false);

        if (SoundManager.Instance)
            SoundManager.Instance.PlaySound2D("Click");
    }


    /// <summary>
    /// Enable or disable all other UI panels in the scene.
    /// </summary>
    private void SetOtherUIPanelsActive(bool active)
    {
        foreach (var panel in otherUIPanels)
        {
            if (panel != null)
                panel.SetActive(active);
        }
    }

    /// <summary>
    /// Call this ONLY when closing the entire quest log menu.
    /// </summary>
    public void CloseQuestLogMenu()
    {
        questLogPage.SetActive(false);

        foreach (var page in questPages)
            page.SetActive(false);

        // Restore all other UI
        SetOtherUIPanelsActive(true);
    }
}
