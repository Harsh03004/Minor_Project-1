using System.Collections;
using UnityEngine;

public class firePlayerMovement : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private bool isFacingRight = true;
    private float moveInput;
    private bool isAttacking = false;

    // Attack durations
    public float attack1Duration = 0.5f;
    public float attack2Duration = 0.6f;
    public float specialAttackDuration = 0.8f;

    // Fireball spell properties
    //a short fireball
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireballSpeed = 10f;

    // Fire Spell 2 properties
    //fire blast
    public GameObject FireSpell2Prefab;
    public float FireSpellDuration = 1.5f;

    // Disappear timing
    public float disappearDelay = 0.5f;
    public float reappearTime = 1f;

    // Ground Layer
    public LayerMask groundLayer;
    private bool isGrounded = false;

    // Ground detection using a Transform (can be linked to a ground object or reference point)
    public Transform groundTransform;
    public float groundCheckRadius = 0.1f;

    // Define boundaries
    public float leftBoundary = 0f;
    public float rightBoundary = 10f;

    // Skill Button references
    public SkillButton fireSpell1Button;  // Reference to FireSpell1 SkillButton
    public SkillButton fireSpell2Button;  // Reference to FireSpell2
    public SkillButton doubleAttackButton;  // Reference to Double Attack SkillButton
    public SkillButton UltimateAttackButton;  // Reference to Ultimate Attack SkillButton



    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure the skill buttons are assigned (if not assigned via the Inspector)
        if (fireSpell1Button == null)
        {
            fireSpell1Button = GameObject.Find("FireSpell1Button").GetComponent<SkillButton>();
        }

        if (fireSpell2Button == null)
        {
            fireSpell2Button = GameObject.Find("FireSpell2Button").GetComponent<SkillButton>();
        }
    }

    void Update()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundTransform.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            anim.SetBool("jumpUp", false);
            anim.SetBool("jumpDown", false);
        }

        // Fire Spell 1 (Fireball)
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (fireSpell1Button.IsUnlocked)
            {
                ShootFireball();
            }
            else
            {
                Debug.Log("Fire Spell 1 is not unlocked yet. Please unlock it in the skill tree.");
            }
        }

        // Fire Spell 2 (Fire Blast)
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (fireSpell2Button.IsUnlocked)
            {
                StartCoroutine(CastFireSpell2());
            }
            else
            {
                Debug.Log("Fire Spell 2 is not unlocked yet. Please unlock it in the skill tree.");
            }
        }

        // Horizontal movement
        moveInput = Input.GetAxisRaw("Horizontal");
        if (!isAttacking)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            anim.SetBool("run", moveInput != 0);

            if (moveInput > 0 && !isFacingRight) Flip();
            else if (moveInput < 0 && isFacingRight) Flip();

            // Jumping logic
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                anim.SetBool("jumpUp", true);
            }
        }

        // Jump animations
        if (rb.velocity.y > 0) anim.SetBool("jumpUp", true);
        else if (rb.velocity.y < 0) anim.SetBool("jumpDown", true);
        else
        {
            anim.SetBool("jumpUp", false);
            anim.SetBool("jumpDown", false);
        }

        // Attack inputs
        if (Input.GetMouseButtonDown(1) && !isAttacking) // Right mouse button
        {
            if (doubleAttackButton.IsUnlocked)
                StartCoroutine(Attack("attack2", attack2Duration));
            else
                Debug.Log("Double Sword Attack is not unlocked yet. Please unlock it in the skill tree.");
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking) StartCoroutine(Attack("attack1", attack1Duration));

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking)
        {
            if (UltimateAttackButton.IsUnlocked)
                StartCoroutine(Attack("Specialattack", specialAttackDuration));
            else
                Debug.Log("Ultimate Attack is not unlocked yet. Please unlock it in the skill tree.");
        }
            // Clamp player position within level boundaries
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, leftBoundary, rightBoundary), transform.position.y);
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

    private void ShootFireball()
    {
        // Instantiate the fireball at the fire point's position and rotation
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);

        // Get the Rigidbody2D component from the instantiated fireball
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

        // Check if the Rigidbody2D component exists
        if (rb != null)
        {
            // Set the fireball's velocity based on the player's facing direction
            rb.velocity = new Vector2(fireballSpeed * Mathf.Sign(transform.localScale.x), 0);
            Debug.Log("Firing Fire Spell 1 (Fireball)");
        }

        // Adjust the fireball's visual direction to match the player's facing direction
        fireball.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
    }

    private IEnumerator CastFireSpell2()
    {
        // Start both actions at the same time: Player disappears and FireSpell2 is cast
        StartCoroutine(DisappearAndReappear());

        // Instantiate the Fire Spell 2 (Fire Blast) at the fire point's position and rotation
        GameObject fireSpell2 = Instantiate(FireSpell2Prefab, firePoint.position, firePoint.rotation);

        // Play the animation for Fire Spell 2
        Animator spellAnimator = fireSpell2.GetComponent<Animator>();
        if (spellAnimator != null)
        {
            spellAnimator.Play("Pullinwind");  // Play the correct animation
            Debug.Log("Casting Fire Spell 2 (Fire Blast)");
        }

        // Wait for the duration of the Fire Spell 2 to finish
        yield return new WaitForSeconds(FireSpellDuration);
        spriteRenderer.enabled = true;
    }

    private IEnumerator DisappearAndReappear()
    {
        spriteRenderer.enabled = false;  // Make player disappear
        yield return null;  // No wait time, just continue
    }
}
