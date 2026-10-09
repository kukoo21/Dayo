using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal; // Required for 2D lights
using System.Collections; // NEW: Required for Coroutines

public class DiwataFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public float followDistance = 0.8f;
    public float followSpeed = 5f;

    [Header("Visibility Settings")]
    [Tooltip("List of scene names where the Diwata should disappear.")]
    public string[] hiddenScenes = { "CombatScene", "BossFight", "Dungeon" };

    [Header("Light Settings")]
    public Light2D diwataLight; // Assign your 2D Light in Inspector
    private bool lightOn = false;

    // --- NEW UI FLASH VARIABLES ---
    [Header("UI Flash Reference")]
    public UnityEngine.UI.Image diwataFlashOverlay; // Assign the white overlay Image here
    public float uiFlashDuration = 0.15f;
    private Coroutine uiFlashRoutine;
    // ----------------------------

    private Animator anim;
    private PlayerMovement playerMovement;

    private Vector3 lastTargetPos;
    private bool isHidden = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
        else
        {
            Debug.LogError("Player Transform is not assigned to DiwataFollow script!");
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckSceneVisibility(SceneManager.GetActiveScene().name);

        // Ensure light starts off
        if (diwataLight != null)
            diwataLight.enabled = lightOn;

        // NEW: Ensure the UI overlay starts invisible
        if (diwataFlashOverlay != null)
        {
            Color c = diwataFlashOverlay.color;
            c.a = 0f;
            diwataFlashOverlay.color = c;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneVisibility(scene.name);
    }

    private void CheckSceneVisibility(string sceneName)
    {
        foreach (string s in hiddenScenes)
        {
            if (sceneName == s)
            {
                SetHiddenState(true);
                return;
            }
        }

        SetHiddenState(false);
    }

    private void SetHiddenState(bool hide)
    {
        if (anim != null)
            anim.SetBool("isHidden", hide);

        gameObject.SetActive(!hide);
        isHidden = hide;
    }

    private void Update()
    {
        if (player == null || playerMovement == null || isHidden)
            return;

        HandleFollow();
        HandleLightToggle();
        HandleFlashInput(); // NEW: Check for flash input
    }

    private void HandleFollow()
    {
        // Get player direction
        Vector2 lastDir = new Vector2(
            playerMovement.GetComponent<Animator>().GetFloat("LastInputX"),
            playerMovement.GetComponent<Animator>().GetFloat("LastInputY")
        );

        // Target position behind player
        Vector3 targetPos = player.position - (Vector3)(lastDir.normalized * followDistance);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // Flip sprite horizontally
        if (lastDir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (lastDir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        lastTargetPos = transform.position;
    }

    private void HandleLightToggle()
    {
        // Original Keypad 4 toggle logic
        if (Input.GetKeyDown(KeyCode.R) && diwataLight != null)
        {
            if(SoundManager.Instance != null)
                SoundManager.Instance.PlaySound2D("Click");

            lightOn = !lightOn;
            diwataLight.enabled = lightOn;
        }
    }

    // NEW: Method to handle the '4' key press and trigger the UI flash
    private void HandleFlashInput()
    {
        // Using Alpha4 for the top-row number '4'
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Stop and restart the UI flash routine immediately
            if (uiFlashRoutine != null)
                StopCoroutine(uiFlashRoutine);

            uiFlashRoutine = StartCoroutine(PerformUIFlash());
        }
    }

    // NEW: Coroutine to perform the UI flash effect
    private IEnumerator PerformUIFlash()
    {
        if (diwataFlashOverlay == null) yield break;

        // 1. Set to visible/semi-transparent white
        Color c = diwataFlashOverlay.color;
        c.a = 0.6f;
        diwataFlashOverlay.color = c;

        // 2. Fade out
        float elapsed = 0f;
        while (elapsed < uiFlashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / uiFlashDuration;

            c.a = Mathf.Lerp(0.6f, 0f, t); // Fade from 0.6 alpha to 0 alpha
            diwataFlashOverlay.color = c;
            yield return null;
        }

        // 3. Ensure it is fully invisible
        c.a = 0f;
        diwataFlashOverlay.color = c;
    }
}