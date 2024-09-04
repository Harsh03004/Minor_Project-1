using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class playermovement : MonoBehaviour
{ public float walkSpeed = 5f; // Speed of the player while walking
    public float runSpeed = 10f; // Speed of the player while running
    private float currentSpeed; // Current speed of the player based on the mode

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector2 movement; // Store player movement input
    public Animator animator;

    private bool isMoving=true;

    private bool isAnimation=false;

    public spellcasting spellcasting;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player
        currentSpeed = walkSpeed; // Set the initial speed to walking speed
    }

    // Update is called once per frame
    void Update()
    {
        // Get horizontal input for movement (A and D keys)
        float moveX = Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0);
        // Check if the left mouse button is pressed or held down
        // Create a movement vector based on input
        movement = new Vector2(moveX, 0);

        // Get horizontal input for movement (A and D keys)

        // Create a movement vector based on input
        movement = new Vector2(moveX, 0);
        // Check if the Shift key is held down for running
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = runSpeed; // Set speed to run speed
        }
        else
        {
            currentSpeed = walkSpeed; // Set speed to walk speed
        }
        if(isAnimation==true)
        {
            currentSpeed=0;
        }
        else{
            currentSpeed=walkSpeed;
        }
        // Flip the sprite based on the direction of movement
        if (movement.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Flip sprite to face left
        }
        else if (movement.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Flip sprite to face right
        }
        animations();
    }

    // FixedUpdate is called at a fixed interval and is independent of frame rate
    void FixedUpdate()
    {
        // Apply movement to the Rigidbody2D
        rb.velocity = movement * currentSpeed;
    }
    
    void animations()
    {
        if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.D))
        {
            animator.SetBool("walk",true);
        }
        else
        {
            animator.SetBool("walk",false);
        }
        if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D)))
        {
            animator.SetBool("run",true);
        }
        else
        {
            animator.SetBool("run",false);
        }
        if(Input.GetMouseButton(0))
        {
            animator.SetBool("attack1",true);
            isAnimation=true;
        }
        if(Input.GetMouseButton(1))
        {
            animator.SetBool("attack3",true);
            isAnimation=true;
        }
        if(Input.GetKey(KeyCode.Q))
        {
            animator.SetBool("attack2",true);
        }
        if(Input.GetKey(KeyCode.T))
        {
            animator.SetBool("attack2",true);
            spellcasting.anim.SetBool("slash",true);
        }
    }
    public void endattackfunction()
    {
        animator.SetBool("attack1",false);
        animator.SetBool("attack3",false);
        animator.SetBool("attack2",false);
        spellcasting.anim.SetBool("slash",false);
        isAnimation=false;
    }
}




