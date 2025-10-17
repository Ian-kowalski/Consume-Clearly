using UnityEngine;

public class MovementScript : MonoBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    private float speed = 8f;

    [SerializeField] private float jumpingPower = 12f;
    [SerializeField] private float landAcceleration = 40f;
    [SerializeField] private float landDeceleration = 50f;
    [SerializeField] private float airAcceleration = 20f;
    [SerializeField] private float airDeceleration = 30f;

    [Header("Jump Settings")] [SerializeField]
    private float coyoteTime = 0.2f;

    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Ground Check")] [SerializeField]
    private Transform groundCheck;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private float horizontal;
    private bool isFacingRight = true;
    private float targetSpeed;
    private float accelRate;
    private float coyoteTimeCounter;
    private float jumpBufferTimeCounter;

    private PlayerAnimationController animationController;
    
    private void Awake()
    {
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<PlayerAnimationController>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D missing from player!");
            enabled = false;
            return;
        }

        if (animationController == null)
        {
            Debug.LogError("PlayerAnimationController missing from player!");
            enabled = false;
            return;
        }
        
        if (groundCheck == null)
        {
            Debug.LogError("Ground Check reference missing from player! Please set it using SetupGroundCheck.");
            enabled = false;
            return;
        }
    }

    public void SetupGroundCheck(Transform groundCheckTransform)
    {
        groundCheck = groundCheckTransform;
        if (groundCheck == null)
        {
            Debug.LogError("Failed to assign GroundCheck! Movement script will not function correctly.");
        }
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        bool isGrounded = IsGrounded();
        
        // Handle walking and idle animations
        if (isGrounded)
        {
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                animationController.SetWalking(true);
                animationController.SetIdle(false);
            }
            else
            {
                animationController.SetWalking(false);
                animationController.SetIdle(true);
            }
        }

        // Handle jumping animation
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            animationController.TriggerJump();
        }

        // Handle sprite flipping
        animationController.FlipSprite(horizontal);
        
        jump();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Move()
    {
        targetSpeed = horizontal * speed;

        if (!IsGrounded())
        {
            accelRate = (Mathf.Abs(horizontal) > 0.01f) ? airAcceleration : airDeceleration;
        }
        else
        {
            accelRate = (Mathf.Abs(horizontal) > 0.01f) ? landAcceleration : landDeceleration;
        }

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
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

    public void jump()
    {
        // Coyote time logic
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump buffer logic
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimeCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferTimeCounter -= Time.deltaTime;
        }

        jumpAction();
    }

    public void jumpAction()
    {
        if (coyoteTimeCounter > 0f && jumpBufferTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpBufferTimeCounter = 0f;
        }

        // Variable jump height
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

#if UNITY_EDITOR
    public void Test_SetHorizontal(float value)
    {
        horizontal = value;
        Move();
    }
    public void Test_Jump()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        jumpBufferTimeCounter = jumpBufferTime;
        jumpAction();
    }
#endif
}