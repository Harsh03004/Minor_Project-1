using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : MonoBehaviour
{
   Animator anim;
public float moveSpeed = 5f; // Movement speed
public float rollDistance = 5f; // Distance for the roll
public float rollDuration = 0.5f; // Duration of the roll
public float attackDuration = 0.5f; // Duration for the first attack
public float secondAttackDuration = 0.6f; // Duration for the second attack
public float slideDistance = 7f; // Distance for the slide
public float slideDuration = 0.4f; // Duration for the slide
public float slideSpeedMultiplier = 1.5f; // Speed multiplier for the slide

private bool isFacingRight = true; // To track the direction player is facing
private Rigidbody2D rb; // Player's Rigidbody2D component
private float moveInput; // Horizontal input
private bool isRolling = false; // To track if the player is rolling
private bool isAttacking = false; // To track if the player is attacking
private bool isSecondAttacking = false; // To track if the player is performing the second attack
private bool isSliding = false; // To track if the player is sliding

void Start()
{
    anim = GetComponent<Animator>();
    rb = GetComponent<Rigidbody2D>();
}

void Update()
{
    // Get horizontal input (A = -1, D = 1)
    moveInput = Input.GetAxisRaw("Horizontal");

    if (!isRolling && !isAttacking && !isSecondAttacking && !isSliding)  // Ensure no movement when performing other actions
    {
        // Move the player
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
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
            Debug.Log("Mouse Button Pressed: Starting First Attack");
            if (!isRolling && !isAttacking && !isSecondAttacking)
            {
                StartCoroutine(Attack());
            }
        }

        // Check for second attack input (Right mouse button)
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Mouse Button Pressed: Starting Second Attack");
            if (!isRolling && !isAttacking && !isSecondAttacking)
            {
                StartCoroutine(SecondAttack());
            }
        }

        // Check for slide input (X key)
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("X Key Pressed: Starting Slide");
            if (!isRolling && !isSliding && !isAttacking && !isSecondAttacking)
            {
                StartCoroutine(Slide());
            }
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
    Debug.Log("First Attack Animation Triggered");

    yield return new WaitForSeconds(attackDuration);

    anim.SetBool("attack1", false);
    isAttacking = false;
    Debug.Log("First Attack Animation Ended");
}

private IEnumerator SecondAttack()
{
    isSecondAttacking = true;
    anim.SetBool("attack2", true);
    Debug.Log("Second Attack Animation Triggered");

    yield return new WaitForSeconds(secondAttackDuration);

    anim.SetBool("attack2", false);
    isSecondAttacking = false;
    Debug.Log("Second Attack Animation Ended");
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
}