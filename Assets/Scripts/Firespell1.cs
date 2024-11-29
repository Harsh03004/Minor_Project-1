using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firespell1 : MonoBehaviour
{
    public float lifetime = 1f;        // Default lifetime
    public float animationSpeed = 1f;  // Default animation speed (1 is normal speed)
    public int damage = 10;            // Damage dealt by the fire spell
    public float hitRange = 0.5f;      // Range to detect enemies
    public LayerMask enemyLayers;      // Layer mask to identify enemies

    private Animator animator;
    private bool hasHit = false;       // Flag to ensure the fireball hits only once

    void Start()
    {
        // Get the Animator component if it exists
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            // Set the animation speed
            animator.speed = animationSpeed;

            // Trigger the animation (assuming you have a trigger set up in your Animator)
            animator.SetTrigger("Firespell1");  // Replace with your trigger name
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
