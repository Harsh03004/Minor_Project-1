using UnityEngine;

public class GroundEffect : MonoBehaviour
{
    public int damage = 20; // Damage dealt to the enemy
    public float destroyDelay = 2f; // Time after which the arrow gets destroyed if it doesn't hit anything
    public float animationDuration = 0.5f; // Duration of the hit animation

    private bool hasHit = false; // Ensures damage is applied only once
    private Animator animator; // Reference to the Animator component

    private void Start()
    {
        // Automatically destroy the arrow after a delay to prevent memory leaks
        Destroy(gameObject, destroyDelay);

        // Get the Animator component (optional, if the arrow has animations)
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return; // Ensure this logic executes only once

        if (collision.CompareTag("enemy"))
        {
            hasHit = true;

            // Optionally, play an arrow hit animation
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }

            // Wait for the animation to complete before applying damage
            StartCoroutine(ApplyDamageAfterAnimation(collision));
        }
    }

    private System.Collections.IEnumerator ApplyDamageAfterAnimation(Collider2D collision)
    {
        // Wait for the hit animation to finish
        yield return new WaitForSeconds(animationDuration);

        // Apply damage to the enemy
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Destroy the arrow after completing the interaction
        Destroy(gameObject);
    }
}
