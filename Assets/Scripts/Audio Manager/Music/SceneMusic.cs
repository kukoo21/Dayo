using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private string musicName; // Set this in the Inspector

    void Start()
    {
        if (MusicManager.Instance != null && !string.IsNullOrEmpty(musicName))
        {
            MusicManager.Instance.PlayMusic(musicName);
        }
        else
        {
            MusicManager.Instance.StopMusic();
        }
    }
}
