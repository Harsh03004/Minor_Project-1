using System.Collections;
using UnityEngine;

public class LeafPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;           // Movement speed
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    private bool canJump = true;            // Boolean to control if the player can jump
    private bool isCastingSpell = false;    // Flag for casting spells

    public float jumpForce = 7f;
    public GameObject arrowPrefab;          // The arrow prefab to be instantiated
    public GameObject spellPrefab;          // The spell prefab to be instantiated
    public Transform firepoint;             // The point from where the arrow and spell will be fired
    public float arrowSpeed = 10f;          // Speed at which the arrow will travel
    public float spellSpeed = 8f;           // Speed at which the spell will travel
    public float arrowSpawnDelay = 0.5f;    // Delay before the arrow is spawned
    public float attackRange = 1.5f;        // Range of the player's melee attack
    public int attackDamage = 20;           // Damage dealt to the enemy
    public LayerMask enemyLayer;            // Layer for identifying enemies
 
    public float specialAttackDuration = 1f; // Duration of the special attack
    public float specialAttackRadius = 3f;  // Radius of the special attack effect
    public float groundLevel = 0f;          // Ground level Y position, change to match your game setup
    public float groundEffectDuration = 2f; // Duration for the ground effect to stay
    public GameObject groundEffectPrefab;

    // Skill Button references
    public SkillButton leafSpell1Button;  // Reference to LeafSpell1 SkillButton
    public SkillButton leafSpell2Button;  // Reference to LeafSpell2
    public SkillButton doubleAttackButton;  // Reference to Double Attack SkillButton
    public SkillButton UltimateAttackButton;  // Reference to Ultimate Attack SkillButton

    // Ground Layer
    public LayerMask groundLayer;
    private bool isGrounded = false;

    public Transform groundTransform;
    public float groundCheckRadius = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundTransform.position, groundCheckRadius, groundLayer);

   
        // Prevent attack actions while jumping or casting a spell
        if (Input.GetMouseButtonDown(0) && !isAttacking && !isCastingSpell)
        {
            StartCoroutine(Attack("attack1", attackRange, attackDamage));
        }

        if (Input.GetMouseButtonDown(1) && !isAttacking && !isCastingSpell)
        {
            if (doubleAttackButton.IsUnlocked)
            {
                StartCoroutine(Attack("attack2", attackRange, attackDamage));
                StartCoroutine(FireArrowWithDelay());
            }

            else
            {
                Debug.Log("Double Attack is not unlocked yet. Please unlock it in the skill tree.");
            }
        }

        // Check for the 'M' key to cast a spell (only if not attacking or jumping)
        if (Input.GetKeyDown(KeyCode.M) && !isAttacking && !isCastingSpell)
        {
            if (leafSpell1Button.IsUnlocked)
            {
                CastSpell();
            }
            else
            {
                Debug.Log("Leaf Spell 1 is not unlocked yet. Please unlock it in the skill tree.");
            }

        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            if(leafSpell2Button.IsUnlocked)
            {
                SpawnGroundEffectAtEnemy();
            }
            else
            {
                Debug.Log("Leaf Spell 2 is not unlocked yet. Please unlock it in the skill tree.");
            }
        }

        // Special attack
        if (Input.GetKeyDown(KeyCode.X) && !isAttacking && !isCastingSpell)
        {
            if (UltimateAttackButton.IsUnlocked)
                StartCoroutine(SpecialAttack());
            else
                Debug.Log("Ultimate Attack is not unlocked yet. Please unlock it in the skill tree.");
        }

        // Jumping logic
        if (Input.GetKeyDown(KeyCode.Space) && canJump && !isAttacking && !isCastingSpell)
        {
            rb.AddForce(Vector2.up * (jumpForce * 100));
            animator.SetBool("jumpUpp", true);
            animator.SetBool("jumpDownn", false);
        }
        if (!Input.GetKey(KeyCode.Space) && canJump && !isAttacking && !isCastingSpell)
        {
            
            animator.SetBool("jumpUpp", false);
            animator.SetBool("jumpDownn", false);
        }

        // Movement input (not allowed during attack or casting spell)
        if (!isAttacking && !isCastingSpell)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);

            if (movement.x > 0 && !isFacingRight) Flip();
            else if (movement.x < 0 && isFacingRight) Flip();

            animator.SetBool("isMoving", movement.x != 0);
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private IEnumerator Attack(string attackTrigger, float range, int damage)
    {
        isAttacking = true;
        animator.SetBool(attackTrigger, true);

        // Deal damage to enemies within range
        yield return new WaitForSeconds(0.2f); // Delay before the attack lands
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(firepoint.position, range, enemyLayer);
        foreach (var enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(0.5f); // Adjust based on animation length
        animator.SetBool(attackTrigger, false);
        isAttacking = false;
    }

    private IEnumerator SpecialAttack()
    {
        isAttacking = true;
        animator.SetBool("sp_attack", true);

        yield return new WaitForSeconds(specialAttackDuration * 0.5f); // Delay before applying the effect
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, specialAttackRadius, enemyLayer);
        foreach (var enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(enemyScript.maxHealth); // Instantly kill
            }
        }

        yield return new WaitForSeconds(specialAttackDuration * 0.5f);
        animator.SetBool("sp_attack", false);
        isAttacking = false;
    }

    IEnumerator FireArrowWithDelay()
    {
        yield return new WaitForSeconds(arrowSpawnDelay);

        GameObject arrow = Instantiate(arrowPrefab, firepoint.position, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

        // Set the arrow direction based on the player's facing direction
        arrowRb.velocity = isFacingRight ? Vector2.right * arrowSpeed : Vector2.left * arrowSpeed;
    }

    void CastSpell()
    {
        // Prevent actions during spell casting
        isCastingSpell = true;

        GameObject spell = Instantiate(spellPrefab, firepoint.position, Quaternion.identity);
        Rigidbody2D spellRb = spell.GetComponent<Rigidbody2D>();

        spellRb.velocity = isFacingRight ? Vector2.right * spellSpeed : Vector2.left * spellSpeed;

        StartCoroutine(ResetCastingFlag());
    }
    void SpawnGroundEffectAtEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        if (enemies.Length == 0) return;

        GameObject nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            // Get the ground level position of the enemy
            Vector3 spawnPosition = new Vector3(nearestEnemy.transform.position.x, groundLevel, 0);

            // Instantiate the ground effect prefab at that position
            GameObject groundEffect = Instantiate(groundEffectPrefab, spawnPosition, Quaternion.identity);

            // Destroy the ground effect after a set duration
            Destroy(groundEffect, groundEffectDuration);
        }
    }
    IEnumerator ResetCastingFlag()
    {
        yield return new WaitForSeconds(0.5f);
        isCastingSpell = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, specialAttackRadius); // Visualize special attack radius
    }
}
