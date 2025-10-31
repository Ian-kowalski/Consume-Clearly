using System.Collections;
using NUnit.Framework;
using Player;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class MovementScript_PlayModeTests
    {
        private GameObject player;
        private MovementScript movementScript;
        private Rigidbody2D rb;
        private GameObject ground;
        private Transform groundCheck;

        [SetUp]
        public void SetUp()
        {
            CreateTestScene();
        }

        private void CreateTestScene()
        {
            ground = new GameObject("Ground");
            ground.transform.position = Vector2.zero;
            var groundCollider = ground.AddComponent<BoxCollider2D>();
            groundCollider.size = new Vector2(10, 1);
            ground.layer = LayerMask.NameToLayer("Ground");

            // Create player with all required components first
            player = new GameObject("Player");
            rb = player.AddComponent<Rigidbody2D>();
            var spriteRenderer = player.AddComponent<SpriteRenderer>();
            var animator = player.AddComponent<Animator>();
            var animController = player.AddComponent<PlayerAnimationController>();

            player.transform.position = new Vector2(0, 1);
            rb.gravityScale = 2f;

            // Create ground check before adding MovementScript
            var groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.parent = player.transform;
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;

            // Add MovementScript last, after setting up all dependencies
            LogAssert.Expect(LogType.Error,
                "Ground Check reference missing from player! Please set it using SetupGroundCheck.");
            movementScript = player.AddComponent<MovementScript>();

            movementScript.SetupGroundCheck(groundCheck);

            movementScript.GetType()
                .GetField("groundLayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(movementScript, (LayerMask)LayerMask.GetMask("Ground"));
        }

        [UnityTest]
        public IEnumerator PlayerMovesRightWhenInputIsPositive()
        {
            movementScript.Test_SetHorizontal(1f);
            yield return new WaitForFixedUpdate();
            Assert.Greater(rb.linearVelocity.x, 0.1f, "Player should move right when horizontal input is positive.");
        }

        [UnityTest]
        public IEnumerator PlayerJumpsWhenGroundedAndJumpExecuted()
        {
            player.transform.position = new Vector2(0, 1);
            yield return new WaitForFixedUpdate();

            movementScript.Test_Jump();
            float initialYVelocity = rb.linearVelocity.y;
            float expectedJumpPower = (float)movementScript.GetType()
                .GetField("jumpingPower",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(movementScript);

            Assert.That(initialYVelocity, Is.EqualTo(expectedJumpPower).Within(0.5f),
                "Player should jump with correct vertical velocity when grounded and jump is executed.");
        }

        [UnityTest]
        public IEnumerator PlayerDoesNotJumpWhenNotGroundedAndNoCoyoteTime()
        {
            player.transform.position = new Vector2(0, 5);
            yield return new WaitForSeconds(0.3f);

            movementScript.Test_Jump();
            yield return new WaitForFixedUpdate();

            Assert.LessOrEqual(rb.linearVelocity.y, 0.1f,
                "Player should not jump when not grounded and coyote time expired.");
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(player);
            Object.Destroy(ground);
        }
    }
}