using System.IO;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SaveSystem_PlayModeTests
{
    private const string TestSaveFileName = "Test_SaveData.json";
    private string testSaveDirectory;

    [SetUp]
    public void SetUp()
    {
        // Setup custom save directory
        testSaveDirectory = Path.Combine(Application.persistentDataPath, "TestSaves");
        if (!Directory.Exists(testSaveDirectory))
        {
            Directory.CreateDirectory(testSaveDirectory);
        }

        // Set the custom save directory for SaveSystem
        SaveSystem.SetCustomSaveDirectory(testSaveDirectory);
        SaveSystem.ClearSaveData(TestSaveFileName); // Clear any test save data
    }

    [TearDown]
    public void TearDown()
    {
        // Clear test save data
        SaveSystem.ClearSaveData(TestSaveFileName);

        // Remove the test directory
        if (Directory.Exists(testSaveDirectory))
        {
            Directory.Delete(testSaveDirectory, true);
        }

        // Reset SaveSystem to use the default directory
        SaveSystem.ClearCustomSaveDirectory();
    }

    [UnityTest]
    public IEnumerator SaveSystem_SavesAndLoadsGameCorrectly()
    {
        // Arrange
        var saveData = new SaveData
        {
            GameTime = 100.5f,
            CurrentScene = "TestScene",
            PlayerPosition = new Vector3(1f, 2f, 3f)
        };

        // Act
        SaveSystem.Save(saveData, TestSaveFileName);
        yield return null;

        var loadedData = SaveSystem.Load(TestSaveFileName);

        // Assert
        Assert.IsNotNull(loadedData);
        Assert.AreEqual(saveData.GameTime, loadedData.GameTime);
        Assert.AreEqual(saveData.CurrentScene, loadedData.CurrentScene);
        Assert.AreEqual(saveData.PlayerPosition, loadedData.PlayerPosition);
    }

    [UnityTest]
    public IEnumerator SaveSystem_ExcludesInvalidScenesFromSaving()
    {
        // Arrange
        var invalidScenes = new[] { "Credits", "MainMenu" };

        foreach (var scene in invalidScenes)
        {
            var saveData = new SaveData
            {
                GameTime = 200f,
                CurrentScene = scene,
                PlayerPosition = new Vector3(0f, 0f, 0f)
            };

            SaveSystem.Save(saveData, TestSaveFileName);
            yield return null;

            // Act
            var loadedData = SaveSystem.Load(TestSaveFileName);

            // Assert
            Assert.IsNull(loadedData, $"Data should not be saved or loaded for invalid scene: {scene}");
        }
    }

    [UnityTest]
    public IEnumerator SaveSystem_InvalidSaveFileReturnsNull()
    {
        // Act
        SaveSystem.ClearSaveData(TestSaveFileName);
        yield return null;

        var loadedData = SaveSystem.Load(TestSaveFileName);

        // Assert
        Assert.IsNull(loadedData);
    }
}