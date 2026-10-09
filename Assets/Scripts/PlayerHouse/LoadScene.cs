using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LevelManager.Instance.LoadScene(sceneToLoad, "CrossFade");
        }
    }
}

