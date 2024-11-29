using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerhealth : MonoBehaviour
{
    public int maxHealth = 100;                // Player's maximum health
    public int currentHealth;                 // Current player health
    public GameObject hitSpritePrefab;         // Prefab for the hit sprite
    public float invulnerabilityTime = 0.5f;   // Time during which the player is invulnerable after being hit
    private bool isInvulnerable;               // Indicates if the player is currently invulnerable
    public HealthBar healthBar;                // Reference to the HealthBar script

    void Start()
    {
        currentHealth = maxHealth;             // Initialize player health
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);  // Set the initial max health in the health bar
            healthBar.SetHealth(currentHealth); // Set the initial health in the health bar
        }
    }

    // Method to take damage when hit by an enemy
    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;            // If invulnerable, ignore damage

        currentHealth -= damage;               // Reduce health
        //Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        // Instantiate the hit sprite at the player's position
        GameObject hitSprite = Instantiate(hitSpritePrefab, transform.position, Quaternion.identity);
        Destroy(hitSprite, 0.5f);              // Destroy the hit sprite after 0.5 seconds

        // Trigger invulnerability
        StartCoroutine(Invulnerability());

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar();        // Update the health bar
        }

        if (currentHealth <= 0)
        {
            Die();                              // Call Die() if health drops to 0 or below
        }
    }

    private IEnumerator Invulnerability()
    {
        isInvulnerable = true;                  // Set invulnerability to true
        yield return new WaitForSeconds(invulnerabilityTime); // Wait for the invulnerability time
        isInvulnerable = false;                 // Reset invulnerability
    }

    // Method to handle the player's death
    void Die()
    {
        Debug.Log("Player died!");
        Destroy(gameObject);                    // Destroy the player GameObject
    }

    public void ModifyHealth(int amount)
    {
        int newHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"Health modified: {currentHealth} -> {newHealth}");
        currentHealth = newHealth;

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar();        // Update the health bar
        }
    }

    public bool IsAtMaxHealth()
    {
        return currentHealth >= maxHealth;
    }
}
