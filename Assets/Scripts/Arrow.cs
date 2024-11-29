using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 20; // Damage dealt to the enemy
    public float destroyDelay = 2f; // Time after which the arrow gets destroyed if it doesn't hit anything
    public float hitRange = 0.5f;
    public LayerMask enemyLayers;


    private void Start()
    {
        // Automatically destroy the arrow after a delay to prevent memory leaks
        Destroy(gameObject, destroyDelay);
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

        //// Check if the arrow hits an enemy
        //if (collision.CompareTag("enemy"))
        //{
        //    Enemy enemy = collision.GetComponent<Enemy>();
        //    if (enemy != null)
        //    {
        //        enemy.TakeDamage(damage); // Call the enemy's TakeDamage method
        //    }

        //    // Destroy the arrow after hitting the enemy
        //    Destroy(gameObject);
        //}

        //// Optionally, destroy the arrow on hitting obstacles (e.g., walls)

    }
}
