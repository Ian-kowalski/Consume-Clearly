using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class MovementScriptEditModeTests
{
    private GameObject playerObj;
    private MovementScript movement;
    private Transform groundCheck;

    private const string AssertMessage = "Assertion failed on expression: 'ShouldRunBehaviour()'";
    private const string RigidbodyErrorMessage = "Rigidbody2D missing from player!";
    private const string GroundCheckErrorMessage = "Ground Check reference missing from player! Please set it using SetupGroundCheck.";
    private const string AnimationControllerErrorMessage = "PlayerAnimationController missing from player!";

    [SetUp]
    public void SetUp()
    {
        playerObj = new GameObject("Player");
        playerObj.AddComponent<SpriteRenderer>();
        playerObj.AddComponent<Animator>();
        playerObj.AddComponent<PlayerAnimationController>();
        groundCheck = new GameObject("GroundCheck").transform;
    }

    [TearDown]
    public void TearDown()
    {
        if (playerObj != null)
            Object.DestroyImmediate(playerObj);
        if (groundCheck != null)
            Object.DestroyImmediate(groundCheck.gameObject);
    }

    private void ExpectValidateComponentsAssertion() =>
        LogAssert.Expect(LogType.Assert, AssertMessage);

    private void ExpectMissingRigidbodyLogs()
    {
        ExpectValidateComponentsAssertion();
        LogAssert.Expect(LogType.Error, RigidbodyErrorMessage);
    }

    private void ExpectMissingGroundCheckLogs()
    {
        ExpectValidateComponentsAssertion();
        LogAssert.Expect(LogType.Error, GroundCheckErrorMessage);
    }
    
    private void ExpectMissingAnimationControllerLogs()
    {
        ExpectValidateComponentsAssertion();
        LogAssert.Expect(LogType.Error, AnimationControllerErrorMessage);
    }

    [Test]
    public void SetupGroundCheckAssignsTransform()
    {
        movement = playerObj.AddComponent<MovementScript>();
        movement.SetupGroundCheck(groundCheck);

        var field = typeof(MovementScript).GetField("groundCheck",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.AreEqual(groundCheck, field.GetValue(movement));
    }

    [Test]
    public void ValidateComponentsMissingRigidbodyDisablesScript()
    {
        ExpectMissingRigidbodyLogs();
        movement = playerObj.AddComponent<MovementScript>();

        typeof(MovementScript).GetField("groundCheck",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(movement, groundCheck);

        movement.SendMessage("ValidateComponents", null, SendMessageOptions.DontRequireReceiver);
        Assert.IsFalse(movement.enabled);
    }

    [Test]
    public void ValidateComponentsMissingGroundCheckDisablesScript()
    {
        ExpectMissingGroundCheckLogs();
        playerObj.AddComponent<Rigidbody2D>();
        movement = playerObj.AddComponent<MovementScript>();

        typeof(MovementScript).GetField("groundCheck",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(movement, null);

        movement.SendMessage("ValidateComponents", null, SendMessageOptions.DontRequireReceiver);
        Assert.IsFalse(movement.enabled);
    }
    

    [Test]
    public void ValidateComponentsMissingAnimationControllerDisablesScript()
    {
        ExpectMissingAnimationControllerLogs();
        
        Object.DestroyImmediate(playerObj);
        playerObj = new GameObject("Player");
        playerObj.AddComponent<Rigidbody2D>();
        movement = playerObj.AddComponent<MovementScript>();
        
        typeof(MovementScript).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(movement, groundCheck);

        movement.SendMessage("ValidateComponents", null, SendMessageOptions.DontRequireReceiver);
        Assert.IsFalse(movement.enabled);
    }
}