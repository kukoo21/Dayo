using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitBtn : MonoBehaviour
{
    public void Quit()
    {
        SceneManager.LoadScene("Main Menu");
    }
}

