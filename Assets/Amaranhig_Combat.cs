using UnityEngine;

public class Amaranhig_Combat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint; // Your existing attack point (e.g., above the enemy)
    public float weaponRange;
    public LayerMask playerlayer;

    // KNOCKBACK PARAMETERS (Adjust these in the Inspector)
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.25f;

    // Downward attack offset (Set this to a negative value, e.g., -1.5f)
    public float downwardAttackOffsetY = -1.5f;

    public void Attack()
    {
        Debug.Log("Attacking!");

        // 1. Check the existing (upper) attack point
        Collider2D[] upperHits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerlayer);

        // Try to handle damage/knockback for hits found
        if (HandleKnockbackAndDamage(upperHits))
        {
            return; // Exit if we successfully hit someone (to avoid double checking)
        }

        // --- If no hit above, check below ---

        // 2. Calculate the downward attack position relative to the enemy's center (transform.position)
        Vector2 downwardAttackPosition = (Vector2)transform.position + new Vector2(0, downwardAttackOffsetY);

        // 3. Check the new downward attack point
        Collider2D[] lowerHits = Physics2D.OverlapCircleAll(downwardAttackPosition, weaponRange, playerlayer);

        HandleKnockbackAndDamage(lowerHits);
    }

    // Helper function to handle damage and knockback logic
    private bool HandleKnockbackAndDamage(Collider2D[] hits)
    {
        if (hits.Length > 0)
        {
            Collider2D hit = hits[0];

            // 1. Deal Damage
            hit.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);

            // 2. Apply Knockback
            PlayerMovement playerMovement = hit.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Calculate the direction vector pointing AWAY from the enemy
                Vector2 knockbackDirection = (hit.transform.position - transform.position).normalized;
                playerMovement.ApplyKnockback(knockbackDirection, knockbackForce, knockbackDuration);
            }
            return true;
        }
        return false;
    }

    // Helper for visualization in the editor
    private void OnDrawGizmosSelected()
    {
        // Draw the existing attack range (from the Transform)
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
        }

        // Draw the new downward attack range (from the calculated position)
        Gizmos.color = Color.cyan;
        Vector2 downwardAttackPosition = (Vector2)transform.position + new Vector2(0, downwardAttackOffsetY);
        Gizmos.DrawWireSphere(downwardAttackPosition, weaponRange);
    }

    public void PlayAttackSound()
    {
        SoundManager.Instance.PlaySound2D("Amaranhig_attack");
    }
}