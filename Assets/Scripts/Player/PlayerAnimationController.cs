using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Animation parameter names
    private static readonly string IS_WALKING = "IsWalking";
    private static readonly string TRIGGER_JUMP = "TriggerJump";
    private static readonly string IS_IDLE = "IsIdle";

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Initialize parameters
        SetIdle(true);
        SetWalking(false);
    }

    public void SetWalking(bool isWalking)
    {
        animator.SetBool(IS_WALKING, isWalking);
        if (isWalking)
        {
            SetIdle(false);
        }
    }

    public void TriggerJump()
    {
        animator.SetTrigger(TRIGGER_JUMP);
        SetIdle(false);
    }

    public void SetIdle(bool isIdle)
    {
        animator.SetBool(IS_IDLE, isIdle);
    }

    public void FlipSprite(float horizontalInput)
    {
        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }
}