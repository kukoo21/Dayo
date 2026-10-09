using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAppearance : MonoBehaviour
{
    public RuntimeAnimatorController noMaskAnimator;
    public RuntimeAnimatorController withMaskAnimator;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateAnimator(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateAnimator(scene.name);
    }

    void UpdateAnimator(string sceneName)
    {
        // Early scenes (no mask)
        if (sceneName == "PlayerHouse" || sceneName == "Town")
        {
            animator.runtimeAnimatorController = noMaskAnimator;
        }
        // Later scenes (with mask)
        else
        {
            animator.runtimeAnimatorController = withMaskAnimator;
        }
    }
}
