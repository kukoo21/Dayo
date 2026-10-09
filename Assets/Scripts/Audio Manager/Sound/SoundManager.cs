using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private SoundLibrary sfxLibrary;
    public SoundLibrary SfxLibrary => sfxLibrary;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfx2DSource;              // main UI / generic sounds
    [SerializeField] private AudioSource randomPitchAudioSource;   // secondary for footsteps, etc.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Warn if not manually assigned in Inspector
            if (sfx2DSource == null)
                Debug.LogWarning("[SoundManager] sfx2DSource not assigned in Inspector!");
            if (randomPitchAudioSource == null)
                Debug.LogWarning("[SoundManager] randomPitchAudioSource not assigned in Inspector!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, pos);
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName, bool randomPitch = false)
    {
        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        if (clip == null)
        {
            Debug.LogWarning($"[SoundManager] Sound not found: {soundName}");
            return;
        }

        if (randomPitch && randomPitchAudioSource != null)
        {
            randomPitchAudioSource.pitch = Random.Range(1f, 1.5f);
            randomPitchAudioSource.PlayOneShot(clip);
        }
        else if (sfx2DSource != null)
        {
            sfx2DSource.pitch = 1f; // ensure UI sounds play at normal pitch
            sfx2DSource.PlayOneShot(clip);
        }
    }
}
