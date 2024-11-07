using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovementfinal : MonoBehaviour
{
    Animator anim;
    public float moveSpeed = 5f;                // Movement speed
    public float rollDistance = 5f;             // Distance for the roll
    public float rollDuration = 0.5f;           // Duration of the roll
    public float attackDuration = 0.5f;         // Duration for the first attack
    public float secondAttackDuration = 0.6f;   // Duration for the second attack
    public float slideDistance = 7f;            // Distance for the slide
    public float slideDuration = 0.4f;          // Duration for the slide
    public float slideSpeedMultiplier = 1.5f;   // Speed multiplier for the slide
    public float jumpForce = 10f;               // Force of the jump
    public LayerMask groundLayer;               // Layer for ground detection

    private bool isFacingRight = true;           // To track the direction player is facing
    private Rigidbody2D rb;                      // Player's Rigidbody2D component
    private float moveInput;                     // Horizontal input
    private bool isRolling = false;              // To track if the player is rolling
    private bool isAttacking = false;            // To track if the player is attacking
    private bool isSecondAttacking = false;      // To track if the player is performing the second attack
    private bool isSliding = false;              // To track if the player is sliding
    private bool isGrounded = false;             // To check if player is on the ground

    // Define boundaries
    public float leftBoundary = 0f;              // Left boundary of the level
    public float rightBoundary = 10f;            // Right boundary of the level

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get horizontal input (A = -1, D = 1)
        moveInput = Input.GetAxisRaw("Horizontal");

        if (!isRolling && !isAttacking && !isSecondAttacking && !isSliding) // Ensure no movement when performing other actions
        {
            // Move the player
            Vector2 targetVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            // Clamp the player's movement to prevent moving past the left and right boundaries
            if (transform.position.x <= leftBoundary && moveInput < 0)
            {
                targetVelocity.x = 0; // Prevent movement left if past the left boundary
            }
            else if (transform.position.x >= rightBoundary && moveInput > 0)
            {
                targetVelocity.x = 0; // Prevent movement right if past the right boundary
            }

            rb.velocity = targetVelocity;
            anim.SetBool("run", moveInput != 0);

            // Check if the player is changing direction and flip accordingly
            if (moveInput > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (moveInput < 0 && isFacingRight)
            {
                Flip();
            }

            // Check for roll input (Shift key)
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                StartCoroutine(Roll());
            }

            // Check for attack input (Left mouse button)
            if (Input.GetMouseButtonDown(0))
            {
                if (!isRolling && !isAttacking && !isSecondAttacking)
                {
                    StartCoroutine(Attack());
                }
            }

            // Check for second attack input (Right mouse button)
            if (Input.GetMouseButtonDown(1))
            {
                if (!isRolling && !isAttacking && !isSecondAttacking)
                {
                    StartCoroutine(SecondAttack());
                }
            }

            // Check for slide input (X key)
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (!isRolling && !isSliding && !isAttacking && !isSecondAttacking)
                {
                    StartCoroutine(Slide());
                }
            }

            // Check for jump input (Space key)
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }
        }
        else if (isRolling)
        {
            anim.SetBool("roll", true);
        }
    }

    // This function flips the player on the X-axis
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1; // Invert the player's X scale to flip
        transform.localScale = scaler;

        anim.SetBool("isFlipping", true);
        StartCoroutine(ResetFlipAnimation());
    }

    private IEnumerator ResetFlipAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("isFlipping", false);
    }

    private IEnumerator Roll()
    {
        isRolling = true;
        anim.SetBool("roll", true);

        Vector2 rollDirection = isFacingRight ? Vector2.right : Vector2.left;
        rb.velocity = rollDirection * rollDistance / rollDuration;

        yield return new WaitForSeconds(rollDuration);

        rb.velocity = Vector2.zero;
        isRolling = false;
        anim.SetBool("roll", false);
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetBool("attack1", true);

        // Set Rigidbody to kinematic to prevent jitter
        rb.isKinematic = true;

        yield return new WaitForSeconds(attackDuration);

        anim.SetBool("attack1", false);
        isAttacking = false;

        // Reset Rigidbody to dynamic
        rb.isKinematic = false;
    }

    private IEnumerator SecondAttack()
    {
        isSecondAttacking = true;
        anim.SetBool("attack2", true);

        // Set Rigidbody to kinematic to prevent jitter
        rb.isKinematic = true;

        yield return new WaitForSeconds(secondAttackDuration);

        anim.SetBool("attack2", false);
        isSecondAttacking = false;

        // Reset Rigidbody to dynamic
        rb.isKinematic = false;
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        anim.SetBool("slide", true);

        // Calculate slide direction
        Vector2 slideDirection = isFacingRight ? Vector2.right : Vector2.left;

        // Apply the slide distance, duration, and speed multiplier
        rb.velocity = slideDirection * (slideDistance / slideDuration) * slideSpeedMultiplier;

        yield return new WaitForSeconds(slideDuration);

        rb.velocity = Vector2.zero;
        isSliding = false;
        anim.SetBool("slide", false);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply jump force
        anim.SetLayerWeight(1, 1f); // Set jump layer weight to 1 to enable jump animation
        anim.SetBool("jump", true); // Set jump animation boolean to true
    }

    private IEnumerator ResetJumpAnimation()
    {
        // Wait until the player is grounded again
        while (!isGrounded)
        {
            yield return null; // Wait for the next frame
        }

        anim.SetBool("jump", false); // Reset jump animation boolean
        anim.SetLayerWeight(1, 0f); // Set jump layer weight to 0 to disable jump animation
    }

    // Collision detection for ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player collides with the ground layer, set isGrounded to true
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // If the player leaves the ground layer, set isGrounded to false
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }
}
