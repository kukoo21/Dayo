using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    private float defaultMusic = 0.5f;
    private float defaultSfx = 0.5f;

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", defaultMusic);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", defaultSfx);

        SetMusicVolume(musicSlider.value);
        SetSfxVolume(sfxSlider.value);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicVolume(value);
    }

    public void SetSfxVolume(float value)
    {
        PlayerPrefs.SetFloat("SfxVolume", value);
        if (AudioManager.instance != null)
            AudioManager.instance.SetSfxVolume(value);
    }
}
