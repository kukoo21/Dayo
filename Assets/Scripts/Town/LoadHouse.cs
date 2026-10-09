using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadHouse : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           LevelManager.Instance.LoadScene(sceneToLoad, "CrossFade");
            MusicManager.Instance.StopMusic();
        }
    }
}

