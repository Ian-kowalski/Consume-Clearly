using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus.Components;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AnimationController))]
public class CompanionFollow2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshSurface surface2D;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private bool canJump = true;
    [SerializeField] private float followDistance = 2f;

    [Header("Ground Settings")]
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Settings")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Path Settings")]
    [SerializeField] private float repathDistance = 1.0f;
    [SerializeField] private float stuckCheckDistance = 2.0f;

    private Rigidbody2D rb;
    private NavMeshPath path;
    private AnimationController animationController;

    private int currentCorner = 1;
    private float coyoteTimeCounter;
    private float jumpBufferTimeCounter;
    private Vector3 lastTargetPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<AnimationController>();
        path = new NavMeshPath();

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Transform found = player.transform.Find("TargetPoint");
                if (found != null)
                    target = found;
                else
                    Debug.LogWarning("TargetPoint not found on player prefab!");
            }
            else
            {
                Debug.LogWarning("Player not found in scene!");
            }
        }

        if (target != null)
            lastTargetPosition = target.position;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        lastTargetPosition = target.position;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        // If within follow distance, stop moving and idle
        if (distanceToPlayer <= followDistance)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animationController.SetWalking(false);
            animationController.SetIdle(true);
            return;
        }

        UpdatePathIfNeeded();

        if (path == null || path.corners.Length == 0) return;
        if (currentCorner >= path.corners.Length) return;

        Vector3 nextCorner = path.corners[currentCorner];
        Vector2 direction = (nextCorner - transform.position).normalized;

        // Move horizontally
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Flip sprite
        animationController.FlipSprite(direction.x);

        // Walking or idle animation
        if (Mathf.Abs(direction.x) > 0.1f)
        {
            animationController.SetWalking(true);
            animationController.SetIdle(false);
        }
        else
        {
            animationController.SetWalking(false);
            animationController.SetIdle(true);
        }

        // Handle jump if path requires moving upward
        if (canJump && IsGrounded() && nextCorner.y > transform.position.y + 0.2f)
        {
            Jump();
            animationController.TriggerJump();
        }

        // Progress through corners
        if (Vector2.Distance(transform.position, nextCorner) < 0.2f && currentCorner < path.corners.Length - 1)
        {
            currentCorner++;
        }

        // Handle coyote time
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void UpdatePathIfNeeded()
    {
        if (target == null) return;

        if (Vector3.Distance(target.position, lastTargetPosition) > repathDistance)
        {
            RecalculatePath();
            lastTargetPosition = target.position;
            return;
        }

        if (path != null && path.corners.Length > 0)
        {
            float distToPath = Vector2.Distance(transform.position, path.corners[Mathf.Min(currentCorner, path.corners.Length - 1)]);
            if (distToPath > stuckCheckDistance)
            {
                RecalculatePath();
            }
        }
    }

    private void RecalculatePath()
    {
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        currentCorner = 1;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // Visualize follow distance
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.position, followDistance);
        }
    }
}
