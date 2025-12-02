using System.Collections;
using UnityEngine;

namespace Player
{
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
        private float targetSpeed;
        private float accelRate;
        private float coyoteTimeCounter;
        private float jumpBufferTimeCounter;
        private bool waitingForJumpAnimation = false;
        private bool jumpApplied = false;

        private AnimationController animationController;

        private void Awake()
        {
            ValidateComponents();
        }

        private void ValidateComponents()
        {
            rb = GetComponent<Rigidbody2D>();
            animationController = GetComponent<AnimationController>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D missing from player!");
                enabled = false;
                return;
            }

            if (animationController == null)
            {
                Debug.LogError("AnimationController missing from player!");
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

            // Remove immediate animation trigger here; we'll trigger and wait from jump logic so the physics jump occurs only after the animation completes.

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
            if (Input.GetKeyDown(KeyCode.Space))
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
                // Only trigger the jump animation and start the coroutine if we're not already waiting for an animation to finish.
                if (!waitingForJumpAnimation)
                {
                    animationController.TriggerJump();
                    Debug.Log("Jump animation triggered");
                    StartCoroutine(ApplyJumpAfterAnimation());
                }

                // clear buffer so we don't retrigger immediately
                jumpBufferTimeCounter = 0f;
            }

            // Variable jump height
            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }

        private IEnumerator ApplyJumpAfterAnimation()
        {
            // Mark that we're waiting for the jump animation to finish so further jumps are blocked.
            waitingForJumpAnimation = true;

            // Apply the jump physics immediately so movement/physics happen at the same time as the animation.
            if (!jumpApplied)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                jumpApplied = true;
                Debug.Log("Jump applied immediately (physics + animation simultaneously)");
            }

            // Wait for the animator to actually enter the Jump state, with a timeout to avoid hanging if something's misconfigured.
            float timeout = 2f;
            float elapsed = 0f;

            while (!animationController.IsInJumpState() && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!animationController.IsInJumpState())
            {
                Debug.LogWarning("Jump animation didn't start in time. Allowing jump state to clear.");
                // Even if animation didn't start, we've already applied the jump physics.
                waitingForJumpAnimation = false;
                yield break;
            }

            // Wait until the jump animation has completed (normalizedTime >= 1) or until timeout
            elapsed = 0f;
            while (animationController.IsInJumpState() && animationController.GetCurrentStateNormalizedTime() < 1f && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Animation finished (or timed out). Allow jumping again.
            waitingForJumpAnimation = false;
            jumpApplied = false; // reset so next jump can be applied

        }

        // This public method can be called by an Animation Event placed at the end of the jump animation
        // to apply the jump exactly when the animation completes.
        public void ApplyJumpFromAnimation()
        {
            if (!waitingForJumpAnimation) return; // only allow if we are expecting a jump
            if (jumpApplied) return; // already applied

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpApplied = true;
            waitingForJumpAnimation = false;
            Debug.Log("Jump applied from Animation Event");
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
}