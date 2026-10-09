using UnityEngine;
using System.Collections.Generic;

public class AOEDamage : MonoBehaviour
{
    [Header("AOE Damage Settings")]
    public int damage = 40;
    public float knockbackForce = 6f;

    [Header("AOE Knockback Duration")] // Added: Separate duration for AOE
    public float knockbackDuration = 0.3f; // Set a longer duration for the AOE feel

    private HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();

    private void OnEnable()
    {
        Debug.Log("AOE: Enabled (collider active). Resetting hit list.");
        hitEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("AOE: Trigger detected with " + other.name);

        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null && !hitEnemies.Contains(enemyHealth))
        {
            Debug.Log("AOE: Damaging " + enemyHealth.name + " for " + damage);
            hitEnemies.Add(enemyHealth);

            enemyHealth.TakeDamage(damage);

            Enemy_movement enemyMove = enemyHealth.GetComponent<Enemy_movement>();
            Rigidbody2D rb = enemyHealth.GetComponent<Rigidbody2D>();

            if (rb != null && enemyMove != null)
            {
                Vector2 knockDir = (enemyHealth.transform.position - transform.position).normalized;
                enemyMove.ApplyKnockback(knockDir, knockbackForce, knockbackDuration);
            }
        }
    }
}