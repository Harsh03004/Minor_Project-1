using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 7f;         // Jump force
    public Transform groundCheck;       // Ground check object
    public float checkRadius = 0.2f;    // Radius for ground detection
    public LayerMask groundLayer;       // Layer to detect ground
    private Rigidbody2D rb;
    private bool isGrounded;            // Is the player grounded?

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // Jump when Space is pressed and the player is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    // Optional: Visualize the ground check in the editor
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
