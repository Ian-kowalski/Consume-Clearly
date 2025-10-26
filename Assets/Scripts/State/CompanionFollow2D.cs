using UnityEngine;
using UnityEngine.AI;
//using NavMeshPlus.Components;


[RequireComponent(typeof(Rigidbody2D))]
public class CompanionFollow2D : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    //public NavMeshSurface surface2D;
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public bool canJump = true;
    
    [Header("Ground Settings")]
    public float groundCheckRadius = 0.1f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    
    private Rigidbody2D rb;
    private NavMeshPath path;
    private int currentCorner = 0;
    private bool isGrounded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        path = new NavMeshPath();
        InvokeRepeating(nameof(UpdatePath), 0f, 0.2f); // refresh path every 0.2s
    }
    void UpdatePath()
    {
        if (target != null)
        {
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
            currentCorner = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        CheckGrounded();

        if (path == null || path.corners.Length == 0) return;

        // Move toward the next path corner
        Vector3 nextCorner = path.corners[currentCorner];
        Vector2 direction = (nextCorner - transform.position).normalized;

        // Horizontal movement
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Vertical (jump) handling
        if (canJump && isGrounded && nextCorner.y > transform.position.y + 0.5f)
        {
            Jump();
        }

        // Progress to next corner if close enough
        if (Vector2.Distance(transform.position, nextCorner) < 0.2f && currentCorner < path.corners.Length - 1)
        {
            currentCorner++;
        }
    }
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void CheckGrounded()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = hit != null;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
