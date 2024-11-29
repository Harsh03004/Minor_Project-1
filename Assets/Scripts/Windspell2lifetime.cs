using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windspell2lifetime : MonoBehaviour
{
    public float lifetime = 1f; // Default lifetime
    public float animationSpeed = 1f; // Default animation speed (1 is normal speed)

    private Animator animator;

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
}
