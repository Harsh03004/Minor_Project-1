using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodSecondspell : MonoBehaviour
{
    public float speed = 10f;
    public float impactDuration = 0.5f; // Duration of the impact animation
    public float hitRange = 0.5f; // Radius of the impact area
    public LayerMask enemyLayers; // Layers to detect enemies
    public int damage = 20; // Damage dealt to enemies

    private Rigidbody2D rb;
    private Animator animator;
    private bool hasImpacted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Set the initial velocity of the projectile
        rb.velocity = (transform.localScale.x > 0 ? Vector2.right : Vector2.left) * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasImpacted && (collision.CompareTag("enemy") || collision.CompareTag("Wall")))
        {
            hasImpacted = true; // Prevent multiple triggers

            // Stop the projectile's movement
            rb.velocity = Vector2.zero;

            // Trigger impact animation
            animator.SetTrigger("Impact");

            // Apply damage to enemies in range
            ApplyDamageToEnemies();

            // Destroy the projectile after the impact animation
            StartCoroutine(DestroyAfterImpact());
        }
    }

    private void ApplyDamageToEnemies()
    {
        // Detect all enemies within the hit range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, hitRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Apply damage to each detected enemy
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
                Debug.Log($"Enemy hit by WoodSecondspell! Damage: {damage}");
            }
        }
    }

    IEnumerator DestroyAfterImpact()
    {
        yield return new WaitForSeconds(impactDuration);
        Destroy(gameObject);
    }

    // Optional: Visualize the hit range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}
