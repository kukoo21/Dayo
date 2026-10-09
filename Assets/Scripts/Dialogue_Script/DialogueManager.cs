using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using System.Linq;

[System.Serializable]
public class DialogueUISet
{
    public CanvasGroup canvasGroup;
    public Image portrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;
    public Button[] choiceButtons;

    // --- NEW: Image for displaying line-specific visuals ---
    public Image dialogueVisualImage; // Assign the Image component here in the Inspector
    // -------------------------------------------------------

    [Header("Display Settings")]
    public bool showPortrait = true;
    public bool showActorName = true;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Sets")]
    public DialogueUISet dialogueUI;
    public DialogueUISet rewardUI;

    [Header("Image Gallery")]
    public ImageGalleryUI imageGalleryUI;

    [Header("Typing Settings")]
    public float typingSpeed = 0.02f;
    public float nextLineDelay = 0.3f;

    [Header("Optional Enemy to Disable")]
    public GameObject enemy;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentSentence;

    private DialogueUISet activeUI;
    public bool isDialogueActive;
    private DialogueSO currentDialogue;
    private int dialogueIndex;
    private bool canAdvance = true;

    public event Action OnDialogueEnded;

    private void Start()
    {
        Instance = this;
        HideUI(dialogueUI);
        HideUI(rewardUI);

        // Make sure any pre-existing images are hidden
        HideDialogueVisuals(dialogueUI);
        HideDialogueVisuals(rewardUI);
    }

    // Helper function to hide the new image component
    private void HideDialogueVisuals(DialogueUISet ui)
    {
        if (ui.dialogueVisualImage != null)
        {
            ui.dialogueVisualImage.gameObject.SetActive(false);
            ui.dialogueVisualImage.sprite = null;
        }
    }

    // NEW: Block starting dialogue if its condition is not met
    public void StartDialogue(DialogueSO dialogueSO, bool useRewardUI = false)
    {
        if (dialogueSO != null && !dialogueSO.isConditionMet())
        {
            ShowRequirementFailed();
            return;
        }

        bool isReward = (useRewardUI || dialogueSO.isRewardDialogue);
        activeUI = isReward ? rewardUI : dialogueUI;

        if (isReward && dialogueSO.rewardIcon != null && rewardUI != null && rewardUI.portrait != null)
        {
            rewardUI.portrait.sprite = dialogueSO.rewardIcon;
            rewardUI.portrait.enabled = true;
        }

        if (activeUI == rewardUI)
            HideUI(dialogueUI);
        else
            HideUI(rewardUI);

        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        isDialogueActive = true;
        ShowDialogue();
    }

    public void AdvanceDialogue()
    {
        if (!canAdvance)
            return;

        if (isTyping)
        {
            StopAllCoroutines();
            activeUI.dialogueText.text = currentSentence;
            isTyping = false;
            return;
        }

        if (dialogueIndex < currentDialogue.lines.Length)
            ShowDialogue();
        else
            ShowChoices();
    }

    private void ShowDialogue()
    {
        DialogueTextLine line = currentDialogue.lines[dialogueIndex];

        if (DialogueHistoryTracker.Instance != null && line.speaker != null)
            DialogueHistoryTracker.Instance.RecordNPC(line.speaker);

        if (activeUI.portrait != null)
        {
            if (line.speaker != null && line.speaker.portrait != null)
            {
                activeUI.portrait.enabled = true;
                activeUI.portrait.sprite = line.speaker.portrait;
            }
            else
            {
                activeUI.portrait.enabled = false;
            }
        }

        if (activeUI.actorName != null)
        {
            if (line.speaker != null && !string.IsNullOrEmpty(line.speaker.actorName))
                activeUI.actorName.text = line.speaker.actorName;
            else
                activeUI.actorName.text = "";
        }

        // --- NEW DIALOGUE IMAGE LOGIC ---
        if (activeUI.dialogueVisualImage != null)
        {
            if (line.dialogueImage != null)
            {
                activeUI.dialogueVisualImage.gameObject.SetActive(true);
                activeUI.dialogueVisualImage.sprite = line.dialogueImage;

                // Adjust size and position based on line settings
                RectTransform rt = activeUI.dialogueVisualImage.rectTransform;
                rt.sizeDelta = line.imageSize;
                rt.anchoredPosition = line.imageOffset;

                // Set image aspect ratio to fit sprite (optional, but recommended)
                activeUI.dialogueVisualImage.SetNativeSize();
            }
            else
            {
                // Hide the image if no sprite is assigned for this line
                activeUI.dialogueVisualImage.gameObject.SetActive(false);
                activeUI.dialogueVisualImage.sprite = null;
            }
        }
        // --------------------------------

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(line.text));
        ClearChoices(activeUI);
        ShowUI(activeUI);
        dialogueIndex++;
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        canAdvance = false;
        currentSentence = sentence;
        activeUI.dialogueText.text = "";

        int letterCounter = 0;
        foreach (char letter in sentence)
        {
            activeUI.dialogueText.text += letter;
            letterCounter++;

            if (letterCounter % 2 == 0 && SoundManager.Instance != null)
                SoundManager.Instance.PlaySound2D("Blip");

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        yield return new WaitForSeconds(nextLineDelay);
        canAdvance = true;

        if (dialogueIndex >= currentDialogue.lines.Length)
            ShowChoices();
    }

    private void ShowChoices()
    {
        ClearChoices(activeUI);

        if (currentDialogue.options.Length > 0)
        {
            if (currentDialogue.options.Length >= 1)
            {
                var option = currentDialogue.options[0];
                activeUI.choiceButtons[0].GetComponentInChildren<TMP_Text>().text = option.optionText;
                activeUI.choiceButtons[0].gameObject.SetActive(true);
                activeUI.choiceButtons[0].onClick.AddListener(() =>
                {
                    PlayClickSound();
                    ChooseOption(option.nextDialogue);
                });
            }

            if (currentDialogue.options.Length >= 2)
            {
                var option = currentDialogue.options[1];
                activeUI.choiceButtons[2].GetComponentInChildren<TMP_Text>().text = option.optionText;
                activeUI.choiceButtons[2].gameObject.SetActive(true);
                activeUI.choiceButtons[2].onClick.AddListener(() =>
                {
                    PlayClickSound();
                    ChooseOption(option.nextDialogue);
                });
            }
        }
        else
        {
            activeUI.choiceButtons[1].GetComponentInChildren<TMP_Text>().text = "OK";
            activeUI.choiceButtons[1].gameObject.SetActive(true);
            activeUI.choiceButtons[1].onClick.AddListener(() =>
            {
                PlayClickSound();
                EndDialogue();
            });
        }
    }

    // NEW: Condition check on option selection
    private void ChooseOption(DialogueSO dialogueSO)
    {
        if (dialogueSO == null)
        {
            EndDialogue();
            return;
        }

        if (!dialogueSO.isConditionMet())
        {
            ShowRequirementFailed();
            return;
        }

        ClearChoices(activeUI);
        StartDialogue(dialogueSO, dialogueSO.isRewardDialogue);
    }

    private void EndDialogue()
    {
        OnDialogueEnded?.Invoke();

        dialogueIndex = 0;
        isDialogueActive = false;
        ClearChoices(activeUI);
        HideUI(activeUI);

        // NEW: Ensure dialogue image is hidden when dialogue ends
        HideDialogueVisuals(activeUI);


        // 2. CHECK FOR POST-DIALOGUE IMAGES (Gallery)
        if (currentDialogue.postDialogueImages != null && currentDialogue.postDialogueImages.Length > 0)
        {
            imageGalleryUI?.ShowGallery(currentDialogue.postDialogueImages.ToList());
        }

        // 3. PROCESS REWARDS/QUESTS
        if (CodexManager.Instance != null && currentDialogue.unlocksCodexEntry)
            CodexManager.Instance.UnlockEntry(currentDialogue.codexEntryID);

        if (currentDialogue.givesItemReward && currentDialogue.rewardItemPrefab != null)
        {
            var inventory = FindFirstObjectByType<InventoryController>();
            if (inventory != null)
                inventory.AddItem(currentDialogue.rewardItemPrefab);
        }

        if (currentDialogue.unlocksQuest && currentDialogue.questToUnlock != null)
        {
            QuestManager.Instance.AddQuest(currentDialogue.questToUnlock);

            if (currentDialogue.questToUnlock.objectives.Exists(o => o.type == ObjectiveType.WinRiddle))
            {
                FindFirstObjectByType<RiddleManager>().StartRiddleQuest();
            }
        }

        // 4. SCENE LOAD / ENEMY DISABLE
        if (!string.IsNullOrEmpty(currentDialogue.sceneToLoad))
        {
            StartCoroutine(LoadSceneAfterDelay(currentDialogue.sceneToLoad, 0.2f));
        }
        if (enemy != null)
            enemy.SetActive(false);
    }


    // NEW: Displays a simple requirement-not-met popup using the same UI
    private void ShowRequirementFailed()
    {
        activeUI = dialogueUI;
        ShowUI(activeUI);
        activeUI.dialogueText.text = "You haven't met the requirements yet.";

        // Hide the dialogue image if showing a generic error
        HideDialogueVisuals(activeUI);

        ClearChoices(activeUI);
        activeUI.choiceButtons[1].GetComponentInChildren<TMP_Text>().text = "OK";
        activeUI.choiceButtons[1].gameObject.SetActive(true);

        activeUI.choiceButtons[1].onClick.AddListener(() =>
        {
            PlayClickSound();
            EndDialogue();
        });

        isDialogueActive = true;
    }

    private void PlayClickSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound2D("Click");
    }

    private void ShowUI(DialogueUISet ui)
    {
        ui.canvasGroup.alpha = 1;
        ui.canvasGroup.interactable = true;
        ui.canvasGroup.blocksRaycasts = true;
    }

    private void HideUI(DialogueUISet ui)
    {
        ui.canvasGroup.alpha = 0;
        ui.canvasGroup.interactable = false;
        ui.canvasGroup.blocksRaycasts = false;
    }

    private void ClearChoices(DialogueUISet ui)
    {
        foreach (var button in ui.choiceButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayClickSound();

            if (isTyping)
            {
                // Finish typing instantly
                StopAllCoroutines();
                activeUI.dialogueText.text = currentSentence;
                isTyping = false;

                // Optional: small delay before allowing advance
                StartCoroutine(EnableAdvanceAfterDelay(nextLineDelay));
            }
            else
            {
                // Advance dialogue or click choice
                if (activeUI.choiceButtons != null && AnyChoiceVisible())
                {
                    ClickDefaultChoice();
                }
                else
                {
                    AdvanceDialogue();
                }
            }
        }
    }

    // Check if any choice button is currently visible
    private bool AnyChoiceVisible()
    {
        foreach (var btn in activeUI.choiceButtons)
        {
            if (btn.gameObject.activeSelf)
                return true;
        }
        return false;
    }

    // Simulate clicking the first visible choice button
    private void ClickDefaultChoice()
    {
        foreach (var btn in activeUI.choiceButtons)
        {
            if (btn.gameObject.activeSelf)
            {
                btn.onClick.Invoke();
                break;
            }
        }
    }

    private IEnumerator EnableAdvanceAfterDelay(float delay)
    {
        canAdvance = false;
        yield return new WaitForSeconds(delay);
        canAdvance = true;
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        LevelManager.Instance.LoadScene(sceneName, "CrossFade");
    }
}