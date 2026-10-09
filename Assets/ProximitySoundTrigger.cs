using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class ProximitySoundTrigger : MonoBehaviour
{
    [Header("Sound Settings")]
    public string soundName = "Hammer";
    public bool loop = true;
    public string playerTag = "Player";

    [Header("Volume & Distance")]
    [Range(0f, 1f)] public float maxVolume = 1f;
    public bool useProximityVolume = true;
    public float fadeOutDuration = 0.5f;

    [Header("Visual Feedback")]
    public GameObject notificationObject;

    private AudioSource activeSource;
    private CircleCollider2D triggerZone;
    private Transform playerTransform;
    private bool isPlayerInside = false;

    [HideInInspector] public bool hasInteracted = false;

    private void Awake()
    {
        triggerZone = GetComponent<CircleCollider2D>();
        triggerZone.isTrigger = true;

        if (notificationObject != null)
            notificationObject.SetActive(false);
    }

    private void Update()
    {
        if (hasInteracted) return;

        if (isPlayerInside && activeSource != null && playerTransform != null && useProximityVolume)
        {
            AdjustVolumeByDistance();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasInteracted) return;
        if (!other.CompareTag(playerTag)) return;

        playerTransform = other.transform;
        isPlayerInside = true;

        if (notificationObject != null)
            notificationObject.SetActive(true);

        if (activeSource == null)
            PlaySound();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        isPlayerInside = false;
        playerTransform = null;

        if (notificationObject != null)
            notificationObject.SetActive(false);

        if (activeSource != null)
        {
            StartCoroutine(FadeAndDestroy(activeSource, fadeOutDuration));
            activeSource = null;
        }
    }

    private void PlaySound()
    {
        if (SoundManager.Instance == null) return;

        AudioClip clip = SoundManager.Instance.SfxLibrary.GetClipFromName(soundName);
        if (clip == null) return;

        activeSource = gameObject.AddComponent<AudioSource>();
        activeSource.clip = clip;
        activeSource.loop = loop;
        activeSource.spatialBlend = 0f;

        activeSource.volume = useProximityVolume ? 0f : maxVolume;
        if (useProximityVolume && playerTransform != null)
            AdjustVolumeByDistance();

        activeSource.Play();
    }

    private void AdjustVolumeByDistance()
    {
        float worldRadius = triggerZone.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // --- INVERTED LOGIC: Far = loud, Near = soft ---
        float volumeFraction = Mathf.Clamp01(distance / worldRadius);

        activeSource.volume = volumeFraction * maxVolume;
    }

    private IEnumerator FadeAndDestroy(AudioSource source, float duration)
    {
        float startVol = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            if (source == null) yield break;

            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, timer / duration);
            yield return null;
        }

        if (source != null)
        {
            source.Stop();
            Destroy(source);
        }
    }

    public void DisableAfterInteraction()
    {
        hasInteracted = true;

        if (notificationObject != null)
            notificationObject.SetActive(false);

        if (activeSource != null)
        {
            StartCoroutine(FadeAndDestroy(activeSource, fadeOutDuration));
            activeSource = null;
        }
    }
}
