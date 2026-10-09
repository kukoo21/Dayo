using UnityEngine;
using UnityEngine.UI;

public class CodexController : MonoBehaviour
{
    [Header("Codex Settings")]
    public GameObject codexPage;
    public GameObject[] creatureButtons;  // Buttons already placed in Canvas

    [Header("Creature Info Pages")]
    public GameObject[] creaturePages;

    void Start()
    {
        InitializeCodex();
    }

    private void InitializeCodex()
    {
        // Assign button listeners to existing buttons (no cloning)
        for (int i = 0; i < creatureButtons.Length; i++)
        {
            int index = i;
            Button btn = creatureButtons[i].GetComponent<Button>();
            btn.onClick.AddListener(() => OnCreatureButtonClicked(index));
        }

        foreach (var page in creaturePages)
            page.SetActive(false);

        codexPage.SetActive(true);
    }

    void OnCreatureButtonClicked(int index)
    {
        codexPage.SetActive(false);

        foreach (var page in creaturePages)
            page.SetActive(false);

        if (index < creaturePages.Length)
            creaturePages[index].SetActive(true);

        if (SoundManager.Instance)
            SoundManager.Instance.PlaySound2D("Click");
    }

    public void ReturnToCodex()
    {
        foreach (var page in creaturePages)
            page.SetActive(false);

        codexPage.SetActive(true);
    }
}
