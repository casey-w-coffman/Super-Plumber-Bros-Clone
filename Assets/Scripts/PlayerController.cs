using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //basic movement forces
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float lowJumpGravityMultiplier = 3f;

    //movement tuning forces
    public float accel = 30f;
    public float turnAccel = 60f; // turn speed is higher
    public float friction = 20f;  //decel
    public float maxSpeed = 8f;

    //set different move states
    public enum MoveState { Idle , Running , Turning }

    float speed;
    MoveState moveState;

    Rigidbody2D rb;
    float velocityY;
    bool isGrounded;
    float moveX;
    bool jumpQueued;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void Update()
    {
        //queue movement on every frame
        var kb = Keyboard.current;
        moveX = 0f;
        if (kb.aKey.isPressed) moveX = -1f;
        if (kb.dKey.isPressed) moveX = 1f;

        if (kb.wKey.wasPressedThisFrame)
            jumpQueued = true;

        //flip the sprite depending on x velocity
        if (speed > 0f) spriteRenderer.flipX = false;
        else if (speed < 0f) spriteRenderer.flipX = true;
    }

    void FixedUpdate()
    {
    //fine tuned movement (turning, acceleration)
    if (moveX != 0f)
    {
        //check if reversing based on velocity
        bool reversing = speed != 0f && Mathf.Sign(moveX) != Mathf.Sign(speed);
        //set move state based on if reversing or not
        moveState = reversing ? MoveState.Turning : MoveState.Running;

        //determine rate to use if turning or not from above
        float rate = reversing ? turnAccel : accel;
        //set max speed
        speed = Mathf.Clamp(speed + moveX * rate * Time.fixedDeltaTime, -maxSpeed, maxSpeed);
    }
    else
    {
        moveState = MoveState.Idle;
        speed = Mathf.MoveTowards(speed, 0f, friction * Time.fixedDeltaTime);
    }

    //basic movement (running, jumping, gravity)
    //if grounded and not moving upward, set velocityY to zero
    if (isGrounded && velocityY < 0) velocityY = 0f;

    //if grounded and wKey pressed, jump
    if (isGrounded && jumpQueued)
        velocityY = jumpForce;

    //set jumpQ'd to false so can't double jump
    jumpQueued = false;

    bool holdingJump = Keyboard.current.wKey.isPressed;

    //use low jump gravity if not holding wKey, gravity if else
    float currentGravity = (velocityY > 0 && !holdingJump)
        ? gravity * lowJumpGravityMultiplier
        : gravity;

    //decrease velocityY by gravity (y=1/x)
    velocityY += currentGravity * Time.fixedDeltaTime;

    rb.linearVelocity = new Vector2(speed, velocityY);
    }     

    //checks if grounded
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.contacts[0].normal.y > 0.5f) isGrounded = true;
    }

    //activated on jump
    void OnCollisionExit2D(Collision2D col)
    {
        isGrounded = false;
    }
}