using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile1 : MonoBehaviour
{
   public float speed = 10f;         // Speed of the fireball
    public float lifetime = 2f;       // Time before the fireball is destroyed

    private void Start()
    {
        // Destroy the fireball after a certain time to save memory
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the fireball forward
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for collision with any object tagged as "Enemy" or "Wall"
        if (collision.CompareTag("Enemy") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject); // Destroy the fireball on impact
        }
    }
}
