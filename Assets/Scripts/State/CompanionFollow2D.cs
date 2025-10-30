using NavMeshPlus.Components;
using UnityEngine.AI;

using UnityEngine;

/// <summary>
/// Controls the companion's movement and pathfinding behavior in a 2D environment.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AnimationController))]
public class CompanionFollow2D : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Target transform to follow (usually the player)")]
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshSurface surface2D;

    [Header("Movement Settings")]
    [Tooltip("Maximum movement speed")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private bool canJump = true;
    [Tooltip("Minimum distance to maintain from target")]
    [SerializeField] private float followDistance = 2f;
    [Tooltip("Distance to look ahead on path for smoother movement")]
    [SerializeField] private float lookAheadDistance = 2f;

    [Header("Ground Settings")]
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Path Settings")]
    [Tooltip("Distance threshold to recalculate path")]
    [SerializeField] private float repathDistance = 1.0f;
    [Tooltip("Distance to check if companion is stuck")]
    [SerializeField] private float stuckCheckDistance = 2.0f;
    [Tooltip("Time before forcing path recalculation when stuck")]
    [SerializeField] private float stuckTimeout = 1.0f;
    [Tooltip("Number of points used for path smoothing")]
    [SerializeField] private int smoothingPoints = 3;

    private Rigidbody2D rb;
    private NavMeshPath path;
    private AnimationController animationController;
    private int currentCorner = 1;
    private float coyoteTimeCounter;
    private Vector3 lastTargetPosition;
    private float stuckTimer;
    private Vector3 lastPosition;
    private Vector3[] smoothedPath;

    /// <summary>
    /// Initializes the companion's components and references.
    /// </summary>
    private void Start()
    {
        InitializeComponents();
        FindTarget();
    }

    /// <summary>
    /// Sets a new target for the companion to follow.
    /// </summary>
    /// <param name="newTarget">The new transform to follow</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        lastTargetPosition = target.position;
        RecalculatePath();
    }

    private void FixedUpdate()
    {
        if (!IsTargetValid()) return;

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer <= followDistance)
        {
            HandleIdleState();
            return;
        }

        UpdatePathfinding();
        HandleMovement();
        CheckIfStuck();
    }

    private void HandleIdleState()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
        animationController.SetWalking(false);
        animationController.SetIdle(true);
    }

    private void UpdatePathfinding()
    {
        if (Vector3.Distance(target.position, lastTargetPosition) > repathDistance)
        {
            RecalculatePath();
            lastTargetPosition = target.position;
        }

        if (path != null && path.corners.Length > 0)
        {
            float distToPath = Vector2.Distance(transform.position,
                path.corners[Mathf.Min(currentCorner, path.corners.Length - 1)]);

            if (distToPath > stuckCheckDistance)
            {
                RecalculatePath();
            }
        }
    }

    private void HandleMovement()
    {
        if (path == null || path.corners.Length <= 1 || currentCorner >= path.corners.Length) return;

        Vector3 nextCorner = GetSmoothedPosition();
        Vector2 direction = (nextCorner - transform.position).normalized;

        // Apply movement
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        animationController.FlipSprite(direction.x);
        UpdateAnimationState(direction);

        // Handle jumping
        if (canJump && IsGrounded() && nextCorner.y > transform.position.y + 0.2f)
        {
            Jump();
        }

        // Progress through path
        if (Vector2.Distance(transform.position, nextCorner) < 0.2f && currentCorner < path.corners.Length - 1)
        {
            currentCorner++;
        }
    }

    private Vector3 GetSmoothedPosition()
    {
        if (currentCorner >= path.corners.Length) return transform.position;

        Vector3 targetPos = path.corners[currentCorner];
        if (smoothedPath == null || smoothedPath.Length == 0) return targetPos;

        int smoothIndex = Mathf.Min(currentCorner, smoothedPath.Length - 1);
        return smoothedPath[smoothIndex];
    }

    private void CheckIfStuck()
    {
        if (Vector3.Distance(transform.position, lastPosition) < 0.1f)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer > stuckTimeout)
            {
                RecalculatePath();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
        lastPosition = transform.position;
    }

    private void RecalculatePath()
    {
        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            currentCorner = 1;
            SmoothPath();
        }
    }

    private void SmoothPath()
    {
        if (path.corners.Length < 2) return;

        smoothedPath = new Vector3[path.corners.Length];
        path.corners.CopyTo(smoothedPath, 0);

        for (int i = 1; i < smoothedPath.Length - 1; i++)
        {
            Vector3 sum = Vector3.zero;
            int count = 0;

            for (int j = Mathf.Max(0, i - smoothingPoints);
                 j < Mathf.Min(smoothedPath.Length, i + smoothingPoints + 1); j++)
            {
                sum += path.corners[j];
                count++;
            }

            smoothedPath[i] = sum / count;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null && path.corners.Length > 0)
        {
            // Draw raw path
            Gizmos.color = Color.red;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
            }

            // Draw smoothed path
            if (smoothedPath != null && smoothedPath.Length > 1)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < smoothedPath.Length - 1; i++)
                {
                    Gizmos.DrawLine(smoothedPath[i], smoothedPath[i + 1]);
                }
            }

            // Draw current target point
            if (currentCorner < path.corners.Length)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(path.corners[currentCorner], 0.3f);
            }
        }

        // Draw ground check and follow distance
        Gizmos.color = Color.yellow;
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        if (target != null)
            Gizmos.DrawWireSphere(target.position, followDistance);
    }

    // Helper methods...
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<AnimationController>();
        path = new NavMeshPath();
        smoothedPath = new Vector3[0];
    }

    private void FindTarget()
    {
        if (target != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Transform found = player.transform.Find("TargetPoint");
            target = found ?? player.transform;
        }
    }

    private bool IsTargetValid()
    {
        return target != null;
    }

    private void UpdateAnimationState(Vector2 direction)
    {
        bool isMoving = Mathf.Abs(direction.x) > 0.1f;
        animationController.SetWalking(isMoving);
        animationController.SetIdle(!isMoving);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animationController.TriggerJump();
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}