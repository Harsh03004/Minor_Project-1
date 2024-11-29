using System.Collections;
using UnityEngine;

public class WindPlayerMovement : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    private bool isCastingSpell = false;
    private Vector2 movement;
    private bool canJump = true;

    // Ground check
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    // Attack properties
    public float attackRange = 1.5f;        // Range of the player's melee attack
    public int attackDamage = 20;
    // Attack durations
    public float attack1Duration = 0.5f;
    public float attack2Duration = 0.6f;

    // Special attack properties
    public float specialAttackRadius = 3f;
    public float specialAttackDuration = 0.8f;

    // wind spell 1 properties
    public GameObject windspell1Prefab;
    public Transform windPoint;
    public float windspell1Speed = 10f;
    public LayerMask enemyLayer;

    // wind spell 2 properties
    public GameObject WindSpell2Prefab;
    public float disappearDelay = 0.5f;
    public float reappearTime = 1f;

    // Skill Button references
    public SkillButton windSpell1Button;  // Reference to FireSpell1 SkillButton
    public SkillButton windSpell2Button;  // Reference to FireSpell2
    public SkillButton doubleAttackButton;  // Reference to Double Attack SkillButton
    public SkillButton UltimateAttackButton;  // Reference to Ultimate Attack SkillButto


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Horizontal movement
        if (!isAttacking && !isCastingSpell)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);

            if (movement.x > 0 && !isFacingRight) Flip();
            else if (movement.x < 0 && isFacingRight) Flip();

            anim.SetBool("run", movement.x != 0);
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && canJump && !isAttacking && !isCastingSpell)
        {
            rb.AddForce(Vector2.up * jumpForce * 100);
            anim.SetBool("jumpUp", true);
            anim.SetBool("jumpDown", false);
        }

        if (!Input.GetKey(KeyCode.Space) && canJump && !isAttacking && !isCastingSpell)
        {
            anim.SetBool("jumpUp", false);
            anim.SetBool("jumpDown", true);
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking) StartCoroutine(Attack("attack1", attack1Duration));

        if (Input.GetMouseButtonDown(1) && !isAttacking)
        {
            if (doubleAttackButton.IsUnlocked)
                StartCoroutine(Attack2("attack2", attackRange, attackDamage));
            else
                Debug.Log("Double attack is not unlocked");
        }

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking)
        {
            if (UltimateAttackButton.IsUnlocked)
                StartCoroutine(SpecialAttack());
            else
                Debug.Log("Ultimate Attack is not unlocked yet. Please unlock it in the skill tree.");
        }

        // Fireball spell
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (windSpell1Button.IsUnlocked)
                ShootWindSpell1();
            else
                Debug.Log("Wind Spell 1 is not unlocked yet. Please unlock it in the skill tree.");
        }

        // Ice spell
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (windSpell2Button.IsUnlocked)
                StartCoroutine(CastWindSpell2());
            else
                Debug.Log("Wind Spell 2 is not unlocked yet. Please unlock it in the skill tree.");
        }
    }

void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private IEnumerator Attack(string attackTrigger, float duration)
    {
        isAttacking = true;
        anim.SetBool(attackTrigger, true);
        yield return new WaitForSeconds(duration);
        anim.SetBool(attackTrigger, false);
        isAttacking = false;
    }

    private IEnumerator Attack2(string attackTrigger, float range, int damage)
    {
        isAttacking = true;
        anim.SetBool(attackTrigger, true);

        // Deal damage to enemies within range
        yield return new WaitForSeconds(0.2f); // Delay before the attack lands
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(windPoint.position, range, enemyLayer);
        foreach (var enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null) enemyScript.TakeDamage(damage);
        }

        yield return new WaitForSeconds(0.5f);
        anim.SetBool(attackTrigger, false);
        isAttacking = false;
    }

    private void ShootWindSpell1()
    {
        GameObject fireball = Instantiate(windspell1Prefab, windPoint.position, windPoint.rotation);
        Projectile fireballScript = fireball.GetComponent<Projectile>();
        fireballScript.speed = windspell1Speed * Mathf.Sign(transform.localScale.x);
        fireball.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
    }

    private IEnumerator SpecialAttack()
    {
        isAttacking = true;
        anim.SetBool("sp_attack", true);

        yield return new WaitForSeconds(specialAttackDuration * 0.5f);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, specialAttackRadius, enemyLayer);
        foreach (var enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null) enemyScript.TakeDamage(enemyScript.maxHealth);
        }

        yield return new WaitForSeconds(specialAttackDuration * 0.5f);
        anim.SetBool("sp_attack", false);
        isAttacking = false;
    }

    private IEnumerator CastWindSpell2()
    {
        GameObject iceSpell = Instantiate(WindSpell2Prefab, windPoint.position, windPoint.rotation);
        Animator spellAnimator = iceSpell.GetComponent<Animator>();
        if (spellAnimator != null) spellAnimator.Play("Pullinwind");

        yield return new WaitForSeconds(disappearDelay);
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(reappearTime);
        spriteRenderer.enabled = true;
    }

    void OnDrawGizmos()
    {
        // Ground check Gizmo
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            float groundCheckSize = isGrounded ? checkRadius : checkRadius * 1.5f;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckSize);
        }

        // Attack range Gizmo
        if (isAttacking)
        {
            Gizmos.color = Color.yellow;
            float attackGizmoSize = attackRange * 1.2f;
            Gizmos.DrawWireSphere(windPoint.position, attackGizmoSize);
        }

        // Special attack radius Gizmo
        if (Input.GetKey(KeyCode.X))
        {
            Gizmos.color = Color.magenta;
            float specialGizmoSize = specialAttackRadius + Mathf.Sin(Time.time * 5f) * 0.5f;
            Gizmos.DrawWireSphere(transform.position, specialGizmoSize);
        }
    }
}