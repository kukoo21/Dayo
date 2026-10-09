using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

[RequireComponent(typeof(Collider2D))]
public class PlayLoopingSoundOnArea : MonoBehaviour
{
    [Header("Sound Settings")]
    [Tooltip("Name of the sound in the SoundLibrary (groupID).")]
    public string soundName = "Hammer";
    public bool loop = true;
    [TagField] // optional custom attribute if you have one
    public string playerTag = "Player";

    [Header("Fade Settings")]
    public bool useFade = true;
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;
    public float targetVolume = 1f;

    private AudioSource activeSource;
    private Coroutine fadeCoroutine;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (activeSource != null)
            return; // already playing

        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("[PlayLoopingSoundOnArea] No SoundManager found!");
            return;
        }

        AudioClip clip = SoundManager.Instance.SfxLibrary.GetClipFromName(soundName);
        if (clip == null)
        {
            Debug.LogWarning($"[PlayLoopingSoundOnArea] Sound '{soundName}' not found in library!");
            return;
        }

        activeSource = gameObject.AddComponent<AudioSource>();
        activeSource.clip = clip;
        activeSource.loop = loop;
        activeSource.spatialBlend = 0f; // 0 = 2D, 1 = 3D positional
        activeSource.volume = useFade ? 0f : targetVolume;
        activeSource.Play();

        if (useFade)
            fadeCoroutine = StartCoroutine(FadeAudio(activeSource, fadeInTime, targetVolume));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (activeSource != null)
        {
            if (useFade)
            {
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeAndStop(activeSource, fadeOutTime));
            }
            else
            {
                activeSource.Stop();
                Destroy(activeSource);
                activeSource = null;
            }
        }
    }

    private IEnumerator FadeAudio(AudioSource source, float duration, float targetVol)
    {
        float startVol = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, targetVol, timer / duration);
            yield return null;
        }

        source.volume = targetVol;
    }

    private IEnumerator FadeAndStop(AudioSource source, float duration)
    {
        float startVol = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, timer / duration);
            yield return null;
        }

        source.Stop();
        Destroy(source);
        activeSource = null;
    }
}
