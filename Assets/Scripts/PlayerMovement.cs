using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;
    public PlayerInputHandler input;
    public FlashlightAim flashlightAim;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 60f;
    public float deceleration = 90f;
    public float airAcceleration = 25f;
    public float airDeceleration = 15f;
    public float upGravityScale = 1f;
    public float downGravityScale = 1.5f;

    [Header("Jump")]
    public float jumpForce = 10f;

    [Header("Ground Checks")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private bool isGrounded;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        CheckGround();

        flashlightAim.SetFacingDirection(input.MoveInput.x);

        if (input.JumpPressed && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float targetSpeed = input.MoveInput.x * moveSpeed;

        float accelRate;

        if (isGrounded)
            accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        else
            accelRate = Mathf.Abs(targetSpeed) > 0.01f ? airAcceleration : airDeceleration;

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);

        if (rb.linearVelocityY > 0.01f)
            rb.gravityScale = upGravityScale;
        else if (rb.linearVelocityY < -0.01f)
            rb.gravityScale = downGravityScale;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }


    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void OnDrawGizmosSelected() //Debug tool
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
