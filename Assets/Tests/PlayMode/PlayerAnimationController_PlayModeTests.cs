using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerAnimationController_PlayModeTests
{
    private GameObject playerObj;
    private PlayerAnimationController animController;
    private Animator animator;
    private RuntimeAnimatorController runtimeAnimatorController;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create the player object with required components
        playerObj = new GameObject("Player");
        animator = playerObj.AddComponent<Animator>();
        playerObj.AddComponent<SpriteRenderer>();
        
        // Load the actual animator controller
        runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Player/PlayerAnimator");
        if (runtimeAnimatorController != null)
            animator.runtimeAnimatorController = runtimeAnimatorController;
            
        animController = playerObj.AddComponent<PlayerAnimationController>();
        
        // Wait a frame for Start to execute
        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
        if (playerObj != null)
            Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator SetWalking_WhenTrue_DisablesIdle()
    {
        animController.SetWalking(true);
        yield return null;
        
        Assert.IsTrue(animator.GetBool("IsWalking"));
        Assert.IsFalse(animator.GetBool("IsIdle"));
    }

    [UnityTest]
    public IEnumerator TriggerJump_TriggersJumpAnimation()
    {
        animController.TriggerJump();
        yield return null;
        
        Assert.IsTrue(animator.GetBool("TriggerJump"));
        Assert.IsFalse(animator.GetBool("IsIdle"));
    }

    [UnityTest]
    public IEnumerator SetIdle_UpdatesAnimatorState()
    {
        // Set walking first
        animController.SetWalking(true);
        yield return null;
        
        // Then set idle
        animController.SetIdle(true);
        yield return null;
        
        Assert.IsTrue(animator.GetBool("IsIdle"));
        Assert.IsFalse(animator.GetBool("IsWalking"));
    }

    [UnityTest]
    public IEnumerator AnimationTransitions_WorkCorrectly()
    {
        // Start with idle
        animController.SetIdle(true);
        yield return new WaitForSeconds(0.1f);
        Assert.IsTrue(animator.GetBool("IsIdle"));

        // Transition to walking
        animController.SetWalking(true);
        yield return new WaitForSeconds(0.1f);
        Assert.IsTrue(animator.GetBool("IsWalking"));
        Assert.IsFalse(animator.GetBool("IsIdle"));

        // Trigger jump
        animController.TriggerJump();
        yield return new WaitForSeconds(0.1f);
        Assert.IsTrue(animator.GetBool("TriggerJump"));
        Assert.IsFalse(animator.GetBool("IsIdle"));

        // Return to idle
        animController.SetWalking(false);
        animController.SetIdle(true);
        yield return new WaitForSeconds(0.1f);
        Assert.IsTrue(animator.GetBool("IsIdle"));
        Assert.IsFalse(animator.GetBool("IsWalking"));
    }
}
