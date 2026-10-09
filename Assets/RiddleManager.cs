using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class RiddleManager : MonoBehaviour
{
    [System.Serializable]
    public class Riddle
    {
        public string question;
        public string answer;
    }

    [Header("UI References")]
    public TextMeshProUGUI riddleText;
    public TMP_InputField answerInput;
    public TextMeshProUGUI wrongCountText;
    public Button submitButton;
    public GameObject riddlePanel;
    public GameObject wrongAnswerImage;
    public GameObject correctAnswerImage;

    [Header("Animations & Audio")]
    public Animator bungisngisAnimator;
    public AudioSource laughSFX;

    [Header("Dead UI")]
    public GameObject deadAnimPanel;
    public Animator deadAnimator;
    public string deadAnimTrigger = "Dead";
    public GameObject gameOverPanel;
    public float uiDelayAfterAnimStart = 1f;

    [Header("Settings")]
    public List<Riddle> riddles = new List<Riddle>();
    public int requiredCorrectAnswers = 3;
    public float imageDisplayTime = 1.5f;

    [Header("Dialogue")]
    public DialogueSO endRiddleDialogue;

    private int wrongAttempts = 0;
    private int correctCount = 0;
    private Riddle currentRiddle;
    private PlayerMovement playerMovement;

    private void Start()
    {
        riddlePanel.SetActive(false);
        wrongAnswerImage.SetActive(false);
        correctAnswerImage.SetActive(false);
        deadAnimPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        submitButton.onClick.AddListener(SubmitAnswer);

        // Enforce string-only input
        answerInput.onValueChanged.AddListener(FilterStringInput);

        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    // Allow only letters (A–Z) and spaces
    private void FilterStringInput(string value)
    {
        string filtered = "";
        foreach (char c in value)
        {
            if (char.IsLetter(c) || c == ' ')
                filtered += c;
        }

        if (filtered != value)
            answerInput.text = filtered;
    }

    public void StartRiddleQuest()
    {
        wrongAttempts = 0;
        correctCount = 0;
        wrongCountText.text = "";

        riddlePanel.SetActive(true);
        wrongAnswerImage.SetActive(false);
        deadAnimPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = false;

        ShowRandomRiddle();
    }

    private void ShowRandomRiddle()
    {
        if (riddles.Count == 0)
        {
            Debug.LogError("No riddles assigned!");
            return;
        }

        Riddle newRiddle;
        do
        {
            newRiddle = riddles[Random.Range(0, riddles.Count)];
        } while (currentRiddle == newRiddle && riddles.Count > 1);

        currentRiddle = newRiddle;
        riddleText.text = currentRiddle.question;
        answerInput.text = "";
        answerInput.ActivateInputField();

        if (bungisngisAnimator != null)
            bungisngisAnimator.Play("Idle");
    }

    private void SubmitAnswer()
    {
        if (string.IsNullOrEmpty(answerInput.text))
            return;

        if (wrongAnswerImage.activeInHierarchy || correctAnswerImage.activeInHierarchy)
            return;

        string playerAnswer = answerInput.text.Trim().ToLower();
        string correctAnswer = currentRiddle.answer.Trim().ToLower();

        if (playerAnswer == correctAnswer)
        {
            correctCount++;
            QuestManager.Instance?.UpdateQuestProgress("WinRiddle", 1);
            SoundManager.Instance?.PlaySound2D("Notify");

            StartCoroutine(ShowCorrectAnswerAnimation());

            if (correctCount >= requiredCorrectAnswers)
            {
                riddlePanel.SetActive(false);

                if (playerMovement != null)
                    playerMovement.enabled = true;

                if (endRiddleDialogue != null && DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.StartDialogue(endRiddleDialogue);
                }

                return;
            }

            return;
        }

        wrongAttempts++;
        wrongCountText.text = "Wrong Guesses: " + wrongAttempts + "/2";

        StartCoroutine(ShowWrongAnswerImage());

        if (bungisngisAnimator != null)
            bungisngisAnimator.SetTrigger("LaughTrigger");

        SoundManager.Instance.PlaySound2D("Bungisngis_laugh");

        if (wrongAttempts >= 2)
        {
            StartCoroutine(TriggerDeadSequence());
            return;
        }
    }

    private IEnumerator ShowWrongAnswerImage()
    {
        answerInput.enabled = false;
        wrongAnswerImage.SetActive(true);

        yield return new WaitForSeconds(imageDisplayTime);

        wrongAnswerImage.SetActive(false);
        answerInput.enabled = true;

        ShowRandomRiddle();
    }

    private IEnumerator ShowCorrectAnswerAnimation()
    {
        answerInput.enabled = false;
        correctAnswerImage.SetActive(true);

        SoundManager.Instance?.PlaySound2D("CorrectAnswer");

        yield return new WaitForSeconds(imageDisplayTime);

        correctAnswerImage.SetActive(false);
        answerInput.enabled = true;

        ShowRandomRiddle();
    }

    private IEnumerator TriggerDeadSequence()
    {
        SoundManager.Instance.PlaySound2D("GameOver");
        SoundManager.Instance.PlaySound2D("Bungisngis_laugh");

        riddlePanel.SetActive(false);
        answerInput.enabled = false;

        if (playerMovement != null)
            playerMovement.enabled = false;

        deadAnimPanel.SetActive(true);

        if (deadAnimator != null)
        {
            deadAnimator.enabled = true;
            deadAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            deadAnimator.SetTrigger(deadAnimTrigger);
        }

        yield return new WaitForSecondsRealtime(uiDelayAfterAnimStart);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("Player has failed the riddle quest.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitAnswer();
        }
    }
}
