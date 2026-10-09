using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ultimate UI")]
    public UnityEngine.UI.Image ultCooldownImage;
    public TMPro.TMP_Text ultCooldownText;

    [Header("Normal Attack UI")]
    public UnityEngine.UI.Image attackFlashOverlay; // Drag a white image here (set alpha to 0 in editor)
    public float flashDuration = 0.15f; // How fast the flash fades out

    [Header("Animation & Movement")]
    public Animator anim;
    public PlayerMovement playerMovement;
    public float attackDuration = 0.4f;
    public float ultDuration = 1.2f;
    public bool isInCombat = false;
    public Vector2 lastDirection;

    [Header("Scene Restrictions")]
    public string[] disabledScenes = { "PlayerHouse", "Town" };
    private bool canAttack = true;

    [Header("Normal Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.6f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;
    public float attackPointDistance = 0.7f;
    public float diagonalMultiplier = 0.75f;

    [Header("Ultimate Whip Settings")]
    public GameObject whipAOE;

    [Header("Ultimate Cooldown")]
    public float ultCooldown = 5f;
    private float ultCooldownTimer = 0f;

    [Header("Enemy Knockback")]
    public float enemyKnockbackForce = 6f;
    public float enemyKnockbackDuration = 0.2f;

    [Header("Companion Reference")]
    public GameObject diwata;
    private SpriteRenderer diwataRenderer;
    private Coroutine fadeRoutine;

    // Track the flash coroutine so we can restart it if spamming spacebar
    private Coroutine attackFlashRoutine;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckScene(SceneManager.GetActiveScene().name);

        if (diwata != null)
            diwataRenderer = diwata.GetComponent<SpriteRenderer>();

        if (whipAOE != null)
            whipAOE.SetActive(false);

        // Ensure flash overlay starts invisible
        if (attackFlashOverlay != null)
        {
            Color c = attackFlashOverlay.color;
            c.a = 0f;
            attackFlashOverlay.color = c;
        }
    }

    private void Update()
    {
        // ------------------ ULTIMATE COOLDOWN LOGIC ------------------
        if (ultCooldownTimer > 0f)
        {
            ultCooldownTimer -= Time.deltaTime;

            if (ultCooldownText != null)
                ultCooldownText.text = Mathf.Ceil(ultCooldownTimer).ToString();

            if (ultCooldownImage != null)
                ultCooldownImage.fillAmount = ultCooldownTimer / ultCooldown;

            if (ultCooldownTimer <= 0f)
            {
                ultCooldownTimer = 0f;

                if (ultCooldownText != null)
                    ultCooldownText.text = "";

                if (ultCooldownImage != null)
                    ultCooldownImage.fillAmount = 0f;
            }
        }

        // Note: I assumed you are calling Attack() from another script (like PlayerInput).
        // If you want the Spacebar check specifically HERE, uncomment the lines below:

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     // You need to pass the direction here, usually from your PlayerMovement script
        //     Attack(playerMovement.GetLastDirection()); 
        // }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckScene(scene.name);
    }

    private void CheckScene(string sceneName)
    {
        foreach (string s in disabledScenes)
        {
            if (sceneName == s)
            {
                canAttack = false;
                return;
            }
        }
        canAttack = true;
    }

    // ============================================================
    // NORMAL ATTACK
    // ============================================================
    public void Attack(Vector2 lastDirection)
    {
        if (!canAttack) return;

        // TRIGGER THE FLASH HERE
        if (attackFlashOverlay != null)
        {
            if (attackFlashRoutine != null) StopCoroutine(attackFlashRoutine);
            attackFlashRoutine = StartCoroutine(PerformFlash());
        }

        this.lastDirection = lastDirection.normalized == Vector2.zero
            ? this.lastDirection
            : lastDirection.normalized;

        anim.SetFloat("LastInputX", this.lastDirection.x);
        anim.SetFloat("LastInputY", this.lastDirection.y);

        UpdateAttackPointPosition();

        anim.SetBool("isAttacking", true);
        if (playerMovement != null)
            playerMovement.canMove = false;

        StartCoroutine(HandleCombatState(attackDuration));
    }

    // NEW: Flash Coroutine Logic
    private IEnumerator PerformFlash()
    {
        // 1. Set to semi-transparent white (Visible)
        Color c = attackFlashOverlay.color;
        c.a = 0.6f; // Adjust this for brightness (0 to 1)
        attackFlashOverlay.color = c;

        // 2. Fade out
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0.6f, 0f, elapsed / flashDuration);
            attackFlashOverlay.color = c;
            yield return null;
        }

        // 3. Ensure it is fully invisible
        c.a = 0f;
        attackFlashOverlay.color = c;
    }

    private void UpdateAttackPointPosition()
    {
        Vector2 dir = lastDirection;

        if (Mathf.Abs(dir.x) > 0.1f && Mathf.Abs(dir.y) > 0.1f)
            dir *= diagonalMultiplier;

        attackPoint.localPosition = dir * attackPointDistance;
    }

    // ============================================================
    // ULTIMATE ATTACK
    // ============================================================
    public void UseUltimate(Vector2 lastDirection)
    {
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound2D("Latigo");
        }

        if (!canAttack) return;

        if (ultCooldownTimer > 0f)
        {
            return;
        }

        ultCooldownTimer = ultCooldown;

        this.lastDirection = lastDirection.normalized == Vector2.zero
            ? this.lastDirection
            : lastDirection.normalized;

        anim.SetFloat("LastInputX", this.lastDirection.x);
        anim.SetFloat("LastInputY", this.lastDirection.y);

        UpdateAttackPointPosition();

        anim.SetBool("isUlting", true);
        if (playerMovement != null)
            playerMovement.canMove = false;

        StartCoroutine(HandleCombatState(ultDuration));
    }


    private void ActivateWhipAOE()
    {
        if (whipAOE != null)
            whipAOE.SetActive(true);
    }

    private void DeactivateWhipAOE()
    {
        if (whipAOE != null)
            whipAOE.SetActive(false);
    }

    private IEnumerator HandleCombatState(float duration)
    {
        isInCombat = true;
        FadeDiwata(false);

        yield return new WaitForSeconds(duration);

        if (playerMovement != null)
            playerMovement.canMove = true;

        anim.SetBool("isAttacking", false);
        anim.SetBool("isUlting", false);

        isInCombat = false;
        FadeDiwata(true);
    }

    // ============================================================
    // NORMAL ATTACK DAMAGE
    // ============================================================
    private void PerformAttack()
    {
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound2D("Weapon");
        }
        Debug.Log("PerformAttack event triggered");
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hits)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            Enemy_movement enemyMove = enemy.GetComponent<Enemy_movement>();
            Rigidbody2D enemyRb = enemy.attachedRigidbody;

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }

            if (enemyRb != null && enemyMove != null)
            {
                Vector2 direction =
                    (enemy.transform.position - transform.position).normalized;

                enemyMove.ApplyKnockback(direction, enemyKnockbackForce, enemyKnockbackDuration);
            }
        }
    }

    // ============================================================
    // DIWATA FADE
    // ============================================================
    private void FadeDiwata(bool fadeIn)
    {
        if (diwata == null || diwataRenderer == null) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCoroutine(fadeIn));
    }

    private IEnumerator FadeCoroutine(bool fadeIn)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        Color c = diwataRenderer.color;

        if (fadeIn)
            diwata.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            diwataRenderer.color = c;
            yield return null;
        }

        c.a = endAlpha;
        diwataRenderer.color = c;

        if (!fadeIn)
            diwata.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}