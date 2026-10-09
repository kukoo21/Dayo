using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;   // Scene name
    [SerializeField] private string transitionName = "CrossFade"; // Optional
    [SerializeField] private Button button;        // Assign the UI button here

    private void Awake()
    {
        // Auto-assign button if placed on the same GameObject
        if (button == null)
            button = GetComponent<Button>();

        // Register click event
        if (button != null)
            button.onClick.AddListener(LoadSelectedScene);
        else
            Debug.LogError("No Button component found on " + gameObject.name);
    }

    private void LoadSelectedScene()
    {
        // Load the scene through your LevelManager
        LevelManager.Instance.LoadScene(sceneToLoad, transitionName);
    }
}
