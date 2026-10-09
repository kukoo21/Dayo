using UnityEngine;
using System.Collections;

public class Enemy_movement : MonoBehaviour
{
    public float speed;
    public float attackRange = 2;
    public float atttackCooldown = 2;
    public float playerDetectRange = 5;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    [Header("UI")]
    public GameObject notifyIcon; // NEW: Reference to the notify icon (e.g., !)

    private float attackCooldownTimer;
    private int facingDirection = -1;
    private EnemyState enemyState;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    // Added: Knockback state variable
    private bool isKnockedBack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);

        // NEW: Hide the icon at start
        if (notifyIcon != null)
            notifyIcon.SetActive(false);
    }

    void Update()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        // Added: Knockback priority check
        if (isKnockedBack)
        {
            // Do not execute any movement or state changes while knocked back
            return;
        }

        CheckForPlayer();

        if (enemyState == EnemyState.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyState.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Added: PUBLIC FUNCTION TO BE CALLED BY PlayerCombat.cs
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (isKnockedBack) return;

        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        StartCoroutine(ResetKnockback(duration));
    }

    // Added: COROUTINE to handle the knockback timer
    private IEnumerator ResetKnockback(float duration)
    {
        yield return new WaitForSeconds(duration);

        isKnockedBack = false;
        rb.linearVelocity = Vector2.zero;
    }

    void Chase()
    {
        if (player.position.x > transform.position.x && facingDirection == -1 ||
                player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;

            // NEW: Show notify icon when player is detected for the first time
            if (enemyState == EnemyState.Idle)
            {
                if (notifyIcon != null)
                {
                    // Check if the health bar is currently hidden before showing the icon
                    EnemyHealth health = GetComponent<EnemyHealth>();
                    if (health != null && health.healthBarUI != null && !health.healthBarUI.activeSelf)
                    {
                        notifyIcon.SetActive(true);
                    }
                }
            }

            // Check attack range and cooldown ready
            if (Vector2.Distance(transform.position, player.position) < attackRange && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = atttackCooldown;
                ChangeState(EnemyState.Attacking);
            }

            else if (Vector2.Distance(transform.position, player.position) > attackRange)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);

            // NEW: Hide notify icon when player is lost/out of range
            if (notifyIcon != null)
                notifyIcon.SetActive(false);
        }
    }

    void ChangeState(EnemyState newState)
    {
        //Exit current state
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", false);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", false);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", false);

        //Update state
        enemyState = newState;

        //Udate new animation
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
    }
    public void PlayMoveSound()
    {
        SoundManager.Instance.PlaySound2D("Amaranhig_groan");
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}