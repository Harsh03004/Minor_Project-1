using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class FireSpell2 : MonoBehaviour
{
    public float lifetime = 1f; // Default lifetime
    public float animationSpeed = 1f; // Default animation speed (1 is normal speed)
    public int damage = 50; //damage by the spell

    public float hitRange = 0.5f;
    public LayerMask enemyLayers;

    private Animator animator; //animator of the spell
    private bool hasHit = false;

    void Start()
    {
        // Get the Animator component if it exists
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            // Set the animation speed
            animator.speed = animationSpeed;
        }

        // Destroy the object after the specified lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Only check for collision if the fireball has not hit an enemy yet
        if (!hasHit)
        {
            // Check for collisions in a certain range (radius)
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, hitRange, enemyLayers);

            foreach (var enemy in hitEnemies)
            {
                // Apply damage to the enemy
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    // Apply damage and stop further checks
                    hasHit = true;  // Mark that the fireball has hit an enemy
                    Debug.Log("Applying damage: " + damage);
                    enemyScript.TakeDamage(damage);
                    Destroy(gameObject, lifetime);  // Destroy fireball after hitting
                    break;  // Stop the loop after the first hit
                }
            }
        }
    }
}
