using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;            // Speed of the fireball
    public float lifetime = 2f;         // Time before the fireball is destroyed
    public int damage = 20;             // Damage dealt by the projectile
    public float hitRange = 0.5f;       // Range to detect enemies when fireball collides
    public LayerMask enemyLayers;       // Layer mask to identify enemies

    private void Start()
    {
        // Destroy the projectile after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the fireball forward
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect enemies in range of the hit using OverlapCircle
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, hitRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Apply damage to all detected enemies
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
                Debug.Log("Enemy hit by fireball! Damage: " + damage);
            }
        }

        // Destroy the fireball after hitting an enemy or a wall
        if (collision.CompareTag("enemy") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    // Visualize the hit range of the fireball in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}
