using NavMeshPlus.NavMeshPlus_master.NavMeshComponents.Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace State
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CompanionFollow2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform target;
        [SerializeField] private NavMeshSurface surface2D;
    
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private bool canJump = true;
    
        [Header("Ground Settings")]
        [SerializeField] private float groundCheckRadius = 0.1f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
    
        [Header("Jump Settings")] [SerializeField]
        private float coyoteTime = 0.2f;

        [SerializeField] private float jumpBufferTime = 0.2f;
    
        [Header("Path Settings")] [SerializeField]
        private float repathDistance = 1.0f;
        [SerializeField] private float stuckCheckDistance = 2.0f;
    
        private Rigidbody2D rb;
        private NavMeshPath path;
        private int currentCorner = 1;
        private float coyoteTimeCounter;
        private float jumpBufferTimeCounter;
        private Vector3 lastTargetPosition;
    
    
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
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
    
        void FixedUpdate()
        {
            UpdatePathIfNeeded();

            if (path == null || path.corners.Length == 0) return;
            if (currentCorner >= path.corners.Length) return;

            // Move toward the next path corner
            Vector3 nextCorner = path.corners[currentCorner];
            Vector2 direction = (nextCorner - transform.position).normalized;

            // Horizontal movement
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

            // Vertical (jump) handling
            if (canJump && IsGrounded() && nextCorner.y > transform.position.y + 0.2f)
            {
                Jump();
                jumpBufferTimeCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferTimeCounter -= Time.deltaTime;
            }

            // Progress to next corner if close enough
            if (Vector2.Distance(transform.position, nextCorner) < 0.2f && currentCorner < path.corners.Length - 1)
            {
                currentCorner++;
            }
            // Coyote time logic
            if (IsGrounded())
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
        }
        void Jump()
        {
            if (coyoteTimeCounter > 0f && jumpBufferTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpBufferTimeCounter = 0f;
            }

        }

        void UpdatePathIfNeeded()
        {
            if (target != null)
            {
                // Target moved far enough
                if (Vector3.Distance(target.position, lastTargetPosition) > repathDistance)
                {
                    RecalculatePath();
                    lastTargetPosition = target.position;
                    return;
                }

                // If companion too far from path, repath as recovery
                if (path != null && path.corners.Length > 0)
                {
                    float distToPath = Vector2.Distance(transform.position, path.corners[Mathf.Min(currentCorner, path.corners.Length - 1)]);
                    if (distToPath > stuckCheckDistance)
                    {
                        RecalculatePath();
                    }
                }
            }
        }
    
        void RecalculatePath()
        {
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
            Mathf.Min(1, path.corners.Length - 1);
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    
        void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
