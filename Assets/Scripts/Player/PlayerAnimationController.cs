using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Animation parameter IDs (for better performance)
    private readonly int IsWalking = Animator.StringToHash("isWalking");
    private readonly int IsJumping = Animator.StringToHash("isJumping");
    private readonly int IsIdle = Animator.StringToHash("isIdle");

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetWalking(bool isWalking)
    {
        animator.SetBool(IsWalking, isWalking);
        // When walking, we're not idle
        animator.SetBool(IsIdle, !isWalking && !animator.GetBool(IsJumping));
    }

    public void SetJumping(bool isJumping)
    {
        animator.SetBool(IsJumping, isJumping);
        // When jumping, we're not idle
        animator.SetBool(IsIdle, !isJumping && !animator.GetBool(IsWalking));
    }

    public void FlipSprite(float horizontalInput)
    {
        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }
}