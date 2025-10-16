using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class MovementScriptEditModeTests
{
    [Test]
    public void SetupGroundCheck_AssignsTransform()
    {
        var playerObj = new GameObject("Player");
        var movement = playerObj.AddComponent<MovementScript>();
        var groundCheck = new GameObject("GroundCheck").transform;

        movement.SetupGroundCheck(groundCheck);

        var field = typeof(MovementScript).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.AreEqual(groundCheck, field.GetValue(movement));

        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(groundCheck.gameObject);
    }

    [Test]
    public void ValidateComponents_MissingRigidbody_DisablesScript()
    {
        LogAssert.Expect(LogType.Assert, "Assertion failed on expression: 'ShouldRunBehaviour()'");
        LogAssert.Expect(LogType.Error, "Rigidbody2D missing from player!");

        var playerObj = new GameObject("Player");
        var movement = playerObj.AddComponent<MovementScript>();
        var groundCheck = new GameObject("GroundCheck").transform;
        typeof(MovementScript).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(movement, groundCheck);

        movement.SendMessage("ValidateComponents", null, SendMessageOptions.DontRequireReceiver);
        Assert.IsFalse(movement.enabled);

        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(groundCheck.gameObject);
    }

    [Test]
    public void ValidateComponents_MissingGroundCheck_DisablesScript()
    {
        LogAssert.Expect(LogType.Assert, "Assertion failed on expression: 'ShouldRunBehaviour()'");
        LogAssert.Expect(LogType.Error, "Ground Check reference missing from player! Please set it using SetupGroundCheck.");

        var playerObj = new GameObject("Player");
        playerObj.AddComponent<Rigidbody2D>();
        var movement = playerObj.AddComponent<MovementScript>();
        typeof(MovementScript).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(movement, null);

        movement.SendMessage("ValidateComponents", null, SendMessageOptions.DontRequireReceiver);
        Assert.IsFalse(movement.enabled);

        Object.DestroyImmediate(playerObj);
    }
}
