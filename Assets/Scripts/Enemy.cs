using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 50;             // Enemy's maximum health
    public int attackDamage = 10;          // Damage dealt to the player
    public float attackRange = 1f;         // Range within which the enemy can attack
    public float detectionRange = 5f;      // Range within which the enemy can detect the player
    public float attackCooldown = 1.5f;    // Cooldown between enemy attacks
    public LayerMask playerLayer;          // Layer assigned to the player
    public Transform attackPoint;          // Point from which the attack happens
    public float moveSpeed = 2f;           // Speed at which the enemy moves towards the player
    public float pushBackForce = 5f;       // Force to push the player back when hit
    public float hitStunDuration = 1f;     // Duration the enemy is stunned when hit

    public int currentHealth;              // Current health of the enemy
    private float nextAttackTime = 0f;     // Timer for the next attack
    private Transform player;               // Reference to the player's transform
    private Playerhealth playerHealth;      // Reference to the player's health script
    private Rigidbody2D playerRb;           // Reference to the player's Rigidbody2D
    private Animator animator;              // Reference to the Animator component
    private bool isAttacking = false;       // Bool to track attack state
    private bool isStunned = false;         // Bool to track hit stun state
    private float stunEndTime = 0f;         // Time when the stun will end
    private bool isDead = false;            // Bool to track if the enemy is dead

    private playerattack playerAttackScript;
    public EnemyHealthBar enemyhealthbar;

    void Start()
    {
        // Initialize enemy health
        currentHealth = maxHealth;
        if(enemyhealthbar != null)
        {
            enemyhealthbar.SetMaxHealth(maxHealth);
            enemyhealthbar.SetHealth(maxHealth);
        }
        animator = GetComponent<Animator>();

        // Find the playerattack script in the scene
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerAttackScript = playerObject.GetComponent<playerattack>();
            if (playerAttackScript == null)
            {
                Debug.LogError("playerattack script not found on Player object!");
            }
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    void Update()
    {
        if (isDead)
        {
            // If dead, prevent further actions
            return;
        }

        if (isStunned)
        {
            if (Time.time >= stunEndTime)
            {
                // Stun ended, resume normal behavior
                isStunned = false;
                animator.SetBool("isHit", false); // Stop hit animation
                Debug.Log("Enemy recovered from hit stun.");
            }
            else
            {
                // If still stunned, prevent any further actions
                return;
            }
        }

        DetectPlayer();

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            {
                // Stop moving and attack when in attack range
                StopMoving();
                if (!isAttacking)
                {
                   // Debug.Log("Player is within attack range. Attempting to attack.");
                    isAttacking = true;
                    animator.SetBool("isAttacking", true); // Set attack animation to true
                }
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;  // Reset the attack cooldown
            }
            else if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
            {
                // Move towards the player when within detection range but outside of attack range
                MoveTowardsPlayer();
                isAttacking = false;
                animator.SetBool("isWalking", true); // Set walking animation
            }
        }
        else
        {
            // If no player is detected, stop movement and walking animation
            animator.SetBool("isWalking", false);
        }
    }

    // Method to detect player within a certain range
    void DetectPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, playerLayer);

        if (colliders.Length > 0)
        {
            player = colliders[0].transform;  // Set the player transform when detected
            playerHealth = player.GetComponent<Playerhealth>();  // Get the player's health script
            playerRb = player.GetComponent<Rigidbody2D>();       // Get the player's Rigidbody2D

            if (playerHealth == null || playerRb == null)
            {
                Debug.LogError("Playerhealth or Rigidbody2D component is missing on the detected player!");
            }
            else
            {
               // Debug.Log("Player detected!");
            }
        }
        else
        {
            player = null;  // Reset player when out of detection range
            playerHealth = null;
            playerRb = null;
          //  Debug.Log("Player not in range.");
        }
    }

    // Method to attack the player and apply push-back
    void AttackPlayer()
    {
        if (playerHealth != null && playerRb != null)
        {
            Debug.Log("Attacking the player!");
            playerHealth.TakeDamage(attackDamage);  // Damage the player

            // Calculate the direction to push the player
            Vector2 pushDirection = (player.position - transform.position).normalized;

            // Apply the push-back force
            playerRb.AddForce(pushDirection * pushBackForce, ForceMode2D.Impulse);

           // Debug.Log("Player hit for " + attackDamage + " damage and pushed back.");
        }
        else
        {
            Debug.LogWarning("PlayerHealth or Rigidbody2D is null; cannot deal damage or push back.");
        }
    }

    // Method to move towards the player
    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized; // Get direction to player
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime); // Move towards player
        }
    }

    // Method to stop moving (used when attacking)
    void StopMoving()
    {
        animator.SetBool("isWalking", false); // Stop walking animation when stopping
    }

    // Method to take damage when attacked by the player
    public void TakeDamage(int damage)
    {

        if (isDead) return;  // Prevent taking damage if already dead
        currentHealth -= damage;

        if(enemyhealthbar != null)
        {
            enemyhealthbar.UpdateHealthBar();
        }

        Debug.Log("Enemy took " + damage + " damage. Current health: " + currentHealth);

        // If health drops to 0 or below, trigger death
        if (currentHealth <= 0)
        {
            
            Die();
        }
        else
        {
            // Apply hit stun and stop attacks temporarily
            isStunned = true;
            animator.SetBool("isHit", true);  // Play hit animation
            stunEndTime = Time.time + hitStunDuration; // Set the time when stun ends
            Debug.Log("Enemy is stunned for " + hitStunDuration + " seconds.");
        }
    }

    // Method to handle the enemy's death
    void Die()
    {
        isDead = true;
        Debug.Log("Enemy died!");

        // Add experience to the player
        if (playerAttackScript != null)
        {
            playerAttackScript.AddExperience(10); // Increase experience by 10
        }

        // Trigger death animation
        animator.SetBool("isDead", true); // Play death animation

        // Stop enemy movement, attack, etc.
        StopMoving();
        isAttacking = false;

        // Start coroutine to wait for the animation to finish before destroying the object
        StartCoroutine(DestroyAfterDeathAnimation());
    }

    // Coroutine to wait for death animation to finish before destroying the enemy
    IEnumerator DestroyAfterDeathAnimation()
    {
        // Get the length of the death animation
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        float deathAnimLength = clipInfo[0].clip.length;

        // Wait for the duration of the death animation
        yield return new WaitForSeconds(deathAnimLength);

        // Destroy the enemy GameObject
        Destroy(gameObject);
    }

    // Visualize the detection and attack ranges in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);  // Detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);  // Attack range
    }
}
