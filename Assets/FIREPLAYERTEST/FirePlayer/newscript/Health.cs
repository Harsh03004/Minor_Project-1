using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;                // Player's maximum health
    private int currentHealth;                 // Current player health
    public GameObject hitSpritePrefab;         // Prefab for the hit sprite
    public float invulnerabilityTime = 0.5f;   // Time during which the player is invulnerable after being hit
    private bool isInvulnerable;               // Indicates if the player is currently invulnerable

    void Start()
    {
        currentHealth = maxHealth;             // Initialize player health
    }

    // Method to take damage when hit by an enemy
    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;            // If invulnerable, ignore damage

        currentHealth -= damage;               // Reduce health
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        // Instantiate the hit sprite at the player's position
        GameObject hitSprite = Instantiate(hitSpritePrefab, transform.position, Quaternion.identity);
        Destroy(hitSprite, 0.5f);              // Destroy the hit sprite after 0.5 seconds

        // Trigger invulnerability
        StartCoroutine(Invulnerability());

        if (currentHealth <= 0)
        {
            Die();                             // Call Die() if health drops to 0 or below
        }
    }

    private IEnumerator Invulnerability()
    {
        isInvulnerable = true;                 // Set invulnerability to true
        yield return new WaitForSeconds(invulnerabilityTime); // Wait for the invulnerability time
        isInvulnerable = false;                // Reset invulnerability
    }

    // Method to handle the player's death
    void Die()
    {
        Debug.Log("Player died!");
        Destroy(gameObject);                   // Destroy the player GameObject
    }

    // Detect collision with the enemy
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // Check if collided with an enemy
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                TakeDamage(enemy.attackDamage); // Use enemy's attack damage
            }
        }
    }
}
