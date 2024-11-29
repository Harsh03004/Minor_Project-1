using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 20; // Damage dealt by the fireball
    public float lifetime = 5f; // Fireball lifetime before disappearing

    void Start()
    {
        // Destroy the fireball after its lifetime
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the fireball collided with an enemy
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Apply damage to the enemy
            }

            Destroy(gameObject); // Destroy the fireball on collision
        }
    }
}
