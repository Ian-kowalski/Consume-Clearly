
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class PlayerAnimationControllerEditModeTests
{
    private GameObject playerObj;
    private PlayerAnimationController animController;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SetUp]
    public void SetUp()
    {
        playerObj = new GameObject("Player");
        animator = playerObj.AddComponent<Animator>();
        spriteRenderer = playerObj.AddComponent<SpriteRenderer>();
        animController = playerObj.AddComponent<PlayerAnimationController>();
    }

    [TearDown]
    public void TearDown()
    {
        if (playerObj != null)
            Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void Start_InitializesRequiredComponents()
    {
        Assert.NotNull(animator, "Animator should be initialized");
        Assert.NotNull(spriteRenderer, "SpriteRenderer should be initialized");
    }

    [Test]
    public void FlipSprite_PositiveInput_NoFlip()
    {
        animController.FlipSprite(1f);
        Assert.IsFalse(spriteRenderer.flipX);
    }

    [Test]
    public void FlipSprite_NegativeInput_FlipsSprite()
    {
        animController.FlipSprite(-1f);
        Assert.IsTrue(spriteRenderer.flipX);
    }
}
