using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageGalleryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup galleryPanel;
    public Image displayImage;
    public Button nextButton;
    public Button backButton;
    public Button closeButton;

    private List<Sprite> imageList;
    private int currentIndex = 0;

    private void Awake()
    {
        // Add listeners for the buttons
        nextButton.onClick.AddListener(NextImage);
        backButton.onClick.AddListener(PreviousImage);
        closeButton.onClick.AddListener(CloseGallery);

        // Ensure the panel starts hidden
        HideGallery();
    }

    /// <summary>
    /// Starts the image gallery display with a list of sprites.
    /// </summary>
    public void ShowGallery(List<Sprite> images)
    {
        if (images == null || images.Count == 0)
        {
            HideGallery();
            return;
        }

        imageList = images;
        currentIndex = 0;

        galleryPanel.alpha = 1;
        galleryPanel.interactable = true;
        galleryPanel.blocksRaycasts = true;

        UpdateDisplay();
    }

    private void HideGallery()
    {
        galleryPanel.alpha = 0;
        galleryPanel.interactable = false;
        galleryPanel.blocksRaycasts = false;
        imageList = null;
        currentIndex = 0;
    }

    private void UpdateDisplay()
    {
        if (imageList == null || imageList.Count == 0) return;

        displayImage.sprite = imageList[currentIndex];

        // Hide/Show navigation buttons based on current index
        backButton.gameObject.SetActive(currentIndex > 0);
        nextButton.gameObject.SetActive(currentIndex < imageList.Count - 1);
    }

    public void NextImage()
    {
        if (imageList != null && currentIndex < imageList.Count - 1)
        {
            currentIndex++;
            UpdateDisplay();
        }
    }

    public void PreviousImage()
    {
        if (imageList != null && currentIndex > 0)
        {
            currentIndex--;
            UpdateDisplay();
        }
    }

    public void CloseGallery()
    {
        HideGallery();

        // Optional: Re-enable player movement if it was disabled by dialogue
        // This assumes player movement is handled elsewhere (like in EnemyHealth or a GameController)
        // If the DialogueManager controls player movement, you might need a separate event.
        // Since we are adding it as a sequence after EndDialogue, controls should already be re-enabled 
        // via OnDialogueEnded?.Invoke().
    }
}