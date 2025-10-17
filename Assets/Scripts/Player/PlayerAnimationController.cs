using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Animation States
    private string currentState;

    // Animation States
    private static readonly string IDLE = "Idle";
    private static readonly string WALK = "Walk";
    private static readonly string JUMP = "Jump";

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Start with idle
        ChangeAnimationState(IDLE);
    }

    // Call this before changing animations to ensure proper transition
    void ChangeAnimationState(string newState)
    {
        // prevent same animation from interrupting itself
        if (currentState == newState) return;

        // play the animation
        animator.Play(newState);

        // reassign the current state
        currentState = newState;
    }

    public void SetWalking()
    {
        ChangeAnimationState(WALK);
    }

    public void SetJumping()
    {
        ChangeAnimationState(JUMP);
    }

    public void SetIdle()
    {
        ChangeAnimationState(IDLE);
    }

    public void FlipSprite(float horizontalInput)
    {
        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }
}