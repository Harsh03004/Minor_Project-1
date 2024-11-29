using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fALLINGARROWS : MonoBehaviour
{
   private Animator animator;
    private bool hasImpacted = false; // To ensure impact animation plays only once

    void Start()
    {
        animator = GetComponent<Animator>();

        // Confirm Animator component exists
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on falling arrow prefab.");
        }

        // Log to confirm the arrow has a collider component
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError("Collider2D component is missing on falling arrow prefab.");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Arrow hit an object with tag: {collision.gameObject.tag}");

        // Detect collision with the ground object by checking for the "Ground" tag
        if (collision.gameObject.CompareTag("Ground") && !hasImpacted)
        {
            hasImpacted = true;
            Debug.Log("Arrow has collided with the ground. Triggering impact animation.");

            // Trigger impact animation
            animator.SetTrigger("OnImpact");

            // Delay destruction of the arrow until after impact animation completes
            StartCoroutine(DestroyAfterImpact());
        }
    }

    // Coroutine to destroy the arrow after the impact animation
    private IEnumerator DestroyAfterImpact()
    {
        // Wait for the impact animation to complete
        float impactAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(impactAnimationLength);
        Debug.Log("Impact animation complete. Destroying arrow.");
        Destroy(gameObject);
    }
}
