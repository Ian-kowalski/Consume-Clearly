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

    [Test]
    public void SetupGroundCheckAssignsTransform()
    {
        playerObj = new GameObject("Player");
        movement = playerObj.AddComponent<MovementScript>();
        groundCheck = new GameObject("GroundCheck").transform;

        movement.SetupGroundCheck(groundCheck);

        var field = typeof(MovementScript).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.AreEqual(groundCheck, field.GetValue(movement));

        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(groundCheck.gameObject);
    }

    [Test]
    public void ValidateComponentsMissingRigidbodyDisablesScript()
    {
        ExpectMissingRigidbodyLogs();

        playerObj = new GameObject("Player");
        movement = playerObj.AddComponent<MovementScript>();
        groundCheck = new GameObject("GroundCheck").transform;
        typeof(MovementScript).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(movement, groundCheck);

        movement.SendMessage("ValidateComponents", null, SendMessageOptions.DontRequireReceiver);
        Assert.IsFalse(movement.enabled);

        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(groundCheck.gameObject);
    }

    [Test]
    public void ValidateComponentsMissingGroundCheckDisablesScript()
    {
        ExpectMissingGroundCheckLogs();

        playerObj = new GameObject("Player");
        playerObj.AddComponent<Rigidbody2D>();
        movement = playerObj.AddComponent<MovementScript>();
        typeof(MovementScript).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(movement, null);

        movement.SendMessage("ValidateComponents", null, SendMessageOptions.DontRequireReceiver);
        Assert.IsFalse(movement.enabled);

        Object.DestroyImmediate(playerObj);
    }
}
