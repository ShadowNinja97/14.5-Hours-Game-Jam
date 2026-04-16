using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;
    public PlayerInputHandler input;
    public FlashlightAim flashlightAim;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 50f;
    public float deceleration = 60f;

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
        float speedDiff = targetSpeed - rb.linearVelocityX;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;

        float movement = speedDiff * accelRate;

        rb.AddForce(Vector2.right * movement);

        flashlightAim.SetFacingDirection(input.MoveInput.x);
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

    private void OnGizmosSelected() //Debug tool
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
