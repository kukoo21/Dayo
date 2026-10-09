using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicManager : MonoBehaviour
{
    public static SceneMusicManager Instance;

    [System.Serializable]
    public class SceneMusicPair
    {
        public string sceneName;
        public string musicTrack;
    }

    [Header("Scene Music Mapping")]
    public List<SceneMusicPair> sceneMusicList = new List<SceneMusicPair>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Look up the music for this scene
        string track = GetMusicForScene(scene.name);
        if (!string.IsNullOrEmpty(track) && MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic(track);
            Debug.Log($"[SceneMusicManager] Playing '{track}' for scene '{scene.name}'.");
        }
        else
        {
            Debug.Log($"[SceneMusicManager] No music assigned for scene '{scene.name}'.");
        }
    }

    private string GetMusicForScene(string sceneName)
    {
        foreach (var pair in sceneMusicList)
        {
            if (pair.sceneName == sceneName)
                return pair.musicTrack;
        }
        return null;
    }
}
