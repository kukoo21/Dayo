using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Required for using Coroutines

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private Rigidbody2D rb;
    public Vector2 moveInput;
    private Animator animator;
    public PlayerCombat playerCombat;
    public bool canMove = true;

    private bool isKnockedBack = false;

    // **NEW:** SpriteRenderer reference and color variables for hit flash
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color hitFlashColor = Color.white; // Default flash color, change in Inspector!
    public float flashDuration = 0.1f; // How long the sprite stays flashed (per flash)
    public int numberOfFlashes = 2; // How many times it flashes during knockback

    // Footstep system
    private bool playingFootsteps = false;
    private float footstepSpeed = 0.6f;
    private string currentSurface = "Floor";
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCombat.playerMovement = this;
        // **NEW:** Get SpriteRenderer and store original color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        //Stop movement during any dialogue
        if ((CutsceneDialogueManager.Instance != null && CutsceneDialogueManager.Instance.isDialogueActive) ||
            (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive))
        {
            StopFootsteps();
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isRunning", false);
            return;
        }

        //Stop movement if locked by combat OR knockback
        if (!canMove || isKnockedBack)
        {
            StopFootsteps();

            if (!isKnockedBack)
            {
                rb.linearVelocity = Vector2.zero;
            }

            animator.SetBool("isRunning", false);
        }
        else // Player is free to move
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            rb.linearVelocity = moveInput.normalized * speed;

            if (moveInput != Vector2.zero)
            {
                animator.SetBool("isRunning", true);
                animator.SetFloat("InputX", moveInput.x);
                animator.SetFloat("InputY", moveInput.y);
                animator.SetFloat("LastInputX", moveInput.x);
                animator.SetFloat("LastInputY", moveInput.y);

                if (!playingFootsteps)
                    StartFootsteps();
            }
            else
            {
                animator.SetBool("isRunning", false);
                StopFootsteps();
            }
        }

        DetectSurface();

        // --------------------------------------------------------
        // ATTACK INPUT - Prevent combat actions during knockback
        // --------------------------------------------------------

        if (Input.GetKeyDown(KeyCode.Space) && !isKnockedBack)
        {
            Vector2 facing = new Vector2(animator.GetFloat("LastInputX"), animator.GetFloat("LastInputY"));
            playerCombat.Attack(facing);
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isKnockedBack)
        {
            Vector2 facing = new Vector2(animator.GetFloat("LastInputX"), animator.GetFloat("LastInputY"));
            playerCombat.UseUltimate(facing);
        }
    }

    // PUBLIC FUNCTION TO BE CALLED BY ENEMY COMBAT SCRIPT
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (isKnockedBack) return;

        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // **NEW:** Start the visual flash effect alongside knockback
        StartCoroutine(FlashEffect(duration));

        StartCoroutine(ResetKnockback(duration));
    }

    // COROUTINE to handle the knockback timer
    private IEnumerator ResetKnockback(float duration)
    {
        yield return new WaitForSeconds(duration);

        isKnockedBack = false;
        rb.linearVelocity = Vector2.zero;
    }

    // **NEW:** Coroutine for the visual flash effect
    private IEnumerator FlashEffect(float totalDuration)
    {
        if (spriteRenderer == null) yield break; // Exit if no sprite renderer

        float singleFlashDuration = totalDuration / (numberOfFlashes * 2f); // Divide total duration for on/off cycles

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = hitFlashColor; // Flash on
            yield return new WaitForSeconds(singleFlashDuration);
            spriteRenderer.color = originalColor; // Flash off
            yield return new WaitForSeconds(singleFlashDuration);
        }

        spriteRenderer.color = originalColor; // Ensure it ends on original color
    }

    private void DetectSurface()
    {
        Vector2 origin = transform.position + Vector3.down * 0.5f;
        float checkDistance = 0.2f;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, checkDistance, groundLayer);
        Debug.DrawRay(origin, Vector2.down * checkDistance, Color.yellow);

        currentSurface = hit.collider ? hit.collider.tag : "Floor";
    }

    private void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    }

    private void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    private void PlayFootstep()
    {
        string soundName = "Footstep_floor";

        switch (currentSurface)
        {
            case "Grass": soundName = "Footstep_grass"; break;
            case "Dirt": soundName = "Footstep_dirt"; break;
        }

        SoundManager.Instance.PlaySound2D(soundName, true);
    }
}