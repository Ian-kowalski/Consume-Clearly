// filepath: c:\Users\ianko\Consume-Clearly\Assets\Tests\EditMode\GameManager_EditModeTests.cs
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    [TestFixture]
    public class GameManager_EditModeTests
    {
        private GameObject go;
        private GameManager gm;

        [SetUp]
        public void SetUp()
        {
            go = new GameObject("GMTest");
            gm = go.AddComponent<GameManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (go != null) Object.DestroyImmediate(go);
        }

        [Test]
        public void FindSceneBackground_ByName_ReturnsSpriteRenderer()
        {
            var bg = new GameObject("Background");
            var sr = bg.AddComponent<SpriteRenderer>();

            var method = typeof(GameManager).GetMethod("FindSceneBackground", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (SpriteRenderer)method.Invoke(gm, null);

            Assert.AreEqual(sr, result);

            Object.DestroyImmediate(bg);
        }

        [Test]
        public void IsSceneValidForCamera_ReturnsFalseForMainMenu()
        {
            // set private CurrentScene to MainMenu
            typeof(GameManager).GetField("CurrentScene", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(gm, "MainMenu");

            var method = typeof(GameManager).GetMethod("IsSceneValidForCamera", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (bool)method.Invoke(gm, null);
            Assert.IsFalse(result);

            typeof(GameManager).GetField("CurrentScene", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(gm, "Level1");
            result = (bool)method.Invoke(gm, null);
            Assert.IsTrue(result);
        }
    }
}

