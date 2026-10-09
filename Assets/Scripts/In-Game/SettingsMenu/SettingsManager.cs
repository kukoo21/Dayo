using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Sprites for Numbers (0 - 10)")]
    public Sprite[] numberSprites; // Assign your 0–10 sprites here

    [Header("Sound Settings")]
    public Image soundVolumeLabel;
    private int soundVolume = 5;

    [Header("Music Settings")]
    public Image musicVolumeLabel;
    private int musicVolume = 5;

    private void Start()
    {
        UpdateSoundLabel();
        UpdateMusicLabel();
    }

    public void ChangeSoundVolume(int amount)
    {
        soundVolume = Mathf.Clamp(soundVolume + amount, 0, 10);
        UpdateSoundLabel();
        // TODO: Apply sound volume in AudioMixer
    }

    public void ChangeMusicVolume(int amount)
    {
        musicVolume = Mathf.Clamp(musicVolume + amount, 0, 10);
        UpdateMusicLabel();
        // TODO: Apply music volume in AudioMixer
    }

    private void UpdateSoundLabel()
    {
        soundVolumeLabel.sprite = numberSprites[soundVolume];
    }

    private void UpdateMusicLabel()
    {
        musicVolumeLabel.sprite = numberSprites[musicVolume];
    }

    public void BackToMenu()
    {
        // Example: hide settings panel
        gameObject.SetActive(false);
        // Or load main menu scene:
        // SceneManager.LoadScene("MainMenu");
    }
}
