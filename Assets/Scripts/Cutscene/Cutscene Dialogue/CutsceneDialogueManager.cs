using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutsceneDialogueManager : MonoBehaviour
{
    public static CutsceneDialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public Button nextButton;
    public TextMeshProUGUI nextButtonText;

    private Queue<DialogueLine> lines;

    [Header("Settings")]
    public bool enableTypingEffect = true;
    public float typingSpeed = 0.02f;
    public Animator animator;

    [Header("Text Blip Settings")]
    public bool enableTextBlip = true;
    public int blipEveryNCharacters = 5;

    private bool isTyping = false;
    private string currentSentence;
    public bool isDialogueActive = false;

    // ⬇ NEW — Event triggered when dialogue fully ends
    public System.Action onDialogueFinished;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (nextButton != null)
            nextButton.onClick.AddListener(DisplayNextDialogueLine);
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            nextButton?.onClick.Invoke();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (nextButton != null)
            nextButton.gameObject.SetActive(true);

        if (animator != null)
            animator.Play("show");

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
            lines.Enqueue(dialogueLine);

        UpdateButtonLabel();
        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueArea.text = currentSentence;
            isTyping = false;
            return;
        }

        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        if (characterIcon != null)
            characterIcon.sprite = currentLine.character.icon;

        if (characterName != null)
            characterName.text = currentLine.character.name;

        StopAllCoroutines();

        if (enableTypingEffect)
            StartCoroutine(TypeSentence(currentLine.line));
        else
            dialogueArea.text = currentLine.line;

        UpdateButtonLabel();
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        currentSentence = sentence;
        dialogueArea.text = "";

        int charCount = 0;
        foreach (char letter in sentence)
        {
            dialogueArea.text += letter;
            charCount++;

            if (enableTextBlip && SoundManager.Instance != null && charCount % blipEveryNCharacters == 0)
                SoundManager.Instance.PlaySound2D("Blip");

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void UpdateButtonLabel()
    {
        if (nextButtonText == null)
            return;

        if (lines.Count <= 1)
            nextButtonText.text = "Ok";
        else
            nextButtonText.text = "Next";
    }

    private void EndDialogue()
    {
        isDialogueActive = false;

        if (animator != null)
            animator.Play("hide");

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        // ⬇ NEW — Notify listeners (DialogueActivator, etc.)
        onDialogueFinished?.Invoke();
        onDialogueFinished = null; // clear listeners
    }
}
