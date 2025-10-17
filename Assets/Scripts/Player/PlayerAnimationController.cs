using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Animation trigger parameter IDs
    private readonly int TriggerWalk = Animator.StringToHash("TriggerWalk");
    private readonly int TriggerJump = Animator.StringToHash("TriggerJump");
    private readonly int TriggerIdle = Animator.StringToHash("TriggerIdle");

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetWalking()
    {
        animator.SetTrigger(TriggerWalk);
    }

    public void SetJumping()
    {
        animator.SetTrigger(TriggerJump);
    }

    public void SetIdle()
    {
        animator.SetTrigger(TriggerIdle);
    }

    public void FlipSprite(float horizontalInput)
    {
        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }
}