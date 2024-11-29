using System.Collections;
using UnityEngine;

public class MOVEMENT : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private bool isFacingRight = true;
    private float moveInput;
    private bool isAttacking = false;

    // Ground check
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    // Jump control
    private bool canDoubleJump = true;
    private bool isJumping = false;

    // Attack durations
    public float attack1Duration = 0.5f;
    public float attack2Duration = 0.6f;
    public float specialAttackDuration = 0.8f;

    // Fireball spell properties
    public GameObject fireballPrefab;
    public Transform firePoint; // Where the fireball will spawn
    public float fireballSpeed = 10f;

    public GameObject WindSpell2Prefab;

    public float castDuration = 1f;

    public Transform fireTornadoPoint;

    public GameObject FiretornadoPrefab;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.N))
        {
            CastIceSpell();
        }

        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded)
        {
            isJumping = false;
            canDoubleJump = true;
        }

        // Get horizontal input (A = -1, D = 1)
        moveInput = Input.GetAxisRaw("Horizontal");

        if (!isAttacking)
        {
            // Move the player
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            anim.SetBool("run", moveInput != 0);

            // Flip the player when changing direction
            if (moveInput > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (moveInput < 0 && isFacingRight)
            {
                Flip();
            }

            // Jumping logic
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    isJumping = true;
                    anim.SetBool("jumpUp", true);
                }
                else if (canDoubleJump && !isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    canDoubleJump = false;
                    anim.SetBool("jumpUp", true);
                }
            }
        }

        // Cast the Fire Tornado using fireTornadoPoint
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            StartCoroutine(CastFireTornado());
        }


        // Handle jump animations based on Rigidbody2D velocity
        if (rb.velocity.y > 0)
        {
            anim.SetBool("jumpUp", true);
            anim.SetBool("jumpDown", false);
        }
        else if (rb.velocity.y < 0)
        {
            anim.SetBool("jumpUp", false);
            anim.SetBool("jumpDown", true);
        }
        else if (isGrounded)
        {
            anim.SetBool("jumpUp", false);
            anim.SetBool("jumpDown", false);
        }

        // Handle attack inputs
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(Attack("attack1", attack1Duration));
        }

        if (Input.GetMouseButtonDown(1) && !isAttacking)
        {
            StartCoroutine(Attack("attack2", attack2Duration));
        }

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking)
        {
            StartCoroutine(Attack("specialAttack", specialAttackDuration));
        }

        // Fireball spell input using M key
        if (Input.GetKeyDown(KeyCode.M))
        {
            ShootFireball();
        }
    }

    // This function flips the player on the X-axis
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    // Function to trigger the attack animations dynamically with custom durations
    private IEnumerator Attack(string attackTrigger, float duration)
    {
        isAttacking = true;
        anim.SetBool(attackTrigger, true);

        yield return new WaitForSeconds(duration);

        anim.SetBool(attackTrigger, false);
        isAttacking = false;
    }

    // Function to shoot the fireball spell
    private void ShootFireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        Projectile fireballScript = fireball.GetComponent<Projectile>();
        fireballScript.speed = fireballSpeed * Mathf.Sign(transform.localScale.x);
        fireball.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
    }

    private void CastIceSpell()
    {
        GameObject iceSpell = Instantiate(WindSpell2Prefab, firePoint.position, firePoint.rotation);
        // Spawn the spell at the player's position and rotation


        // Play the animation on the spell if it has an Animator component
        Animator spellAnimator = iceSpell.GetComponent<Animator>();
        if (spellAnimator != null)
        {
            spellAnimator.Play("Firespell1"); // Replace with your animation clip's name
        }
    }

    private IEnumerator CastFireTornado()
    {
        // Stop player movement
        isAttacking = true;
        rb.velocity = Vector2.zero; // Freeze the player’s movement

        // Instantiate spell at fireTornadoPoint
        GameObject fireTornado = Instantiate(FiretornadoPrefab, fireTornadoPoint.position, fireTornadoPoint.rotation);

        // Play the spell animation if it has an Animator
        Animator spellAnimator = fireTornado.GetComponent<Animator>();
        if (spellAnimator != null)
        {
            spellAnimator.Play("Firespell2"); // Replace with the actual animation clip name
        }

        // Wait for the casting duration
        yield return new WaitForSeconds(castDuration);

        // Allow player movement again
        isAttacking = false;
    }

    // Optional: Draw the ground check circle in the editor for visualization
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
