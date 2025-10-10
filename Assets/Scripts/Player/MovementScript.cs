using UnityEngine;

public class MovementScript : MonoBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    private float speed = 8f;

    [SerializeField] private float jumpingPower = 10f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 30f;

    [Header("Ground Check")] [SerializeField]
    private Transform groundCheck;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private float horizontal;
    private bool isFacingRight = true;
    private float targetSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D missing from player!");
            enabled = false;
            return;
        }

        if (groundCheck == null)
        {
            Debug.LogError("Ground Check reference missing from player!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        // Get input
        horizontal = Input.GetAxisRaw("Horizontal");

        // Jump logic
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        // Variable jump height
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // Handle facing direction
        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        targetSpeed = horizontal * speed;

        if (!IsGrounded())
        {
            // Air control
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            float speedDiff = targetSpeed - rb.linearVelocity.x;
            float movement = speedDiff * accelRate * Time.fixedDeltaTime;
            rb.AddForce(new Vector2(movement, 0f), ForceMode2D.Force);
        }
        else
        {
            // Ground movement
            rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void UpdateFacingDirection()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // Optional: Add getters for animation purposes
    public bool IsMoving() => Mathf.Abs(horizontal) > 0.01f;
    public bool IsFacingRight() => isFacingRight;
    public float GetHorizontalSpeed() => rb.linearVelocity.x;
    public float GetVerticalSpeed() => rb.linearVelocity.y;
}