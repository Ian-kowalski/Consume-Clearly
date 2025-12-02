using UnityEngine;

namespace Player
{
    public class AnimationController : MonoBehaviour
    {
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        // Animation parameter names
        private static readonly string IS_WALKING = "IsWalking";
        private static readonly string TRIGGER_JUMP = "TriggerJump";
        private static readonly string IS_IDLE = "IsIdle";

        [Header("State Names")] [SerializeField]
        private string jumpStateName = "Jump";

        void Start()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        
            if (ValidateComponents()) return;

            // Initialize parameters
            SetIdle(true);
            SetWalking(false);
        }

        private bool ValidateComponents()
        {
            if (animator == null)
            {
                Debug.LogError("Animator component missing from player!");
                enabled = false;
                return true;
            }

            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer component missing from player!");
                enabled = false;
                return true;
            }

            return false;
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
        
        // Helper to detect if the animator is currently in the jump state.
        public bool IsInJumpState()
        {
            if (animator == null) return false;
            var state = animator.GetCurrentAnimatorStateInfo(0);
            if (string.IsNullOrEmpty(jumpStateName)) return state.IsName("Jump");
            return state.IsName(jumpStateName);
        }

        // Returns the normalized time of the current state (0..inf, 1 means the state completed one loop)
        public float GetCurrentStateNormalizedTime()
        {
            if (animator == null) return 0f;
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
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
}