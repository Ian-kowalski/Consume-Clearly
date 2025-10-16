using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SaveSystemPlayModeTests
{
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
        SaveSystem.Save(saveData);
        yield return null;

        var loadedData = SaveSystem.Load();
        
        // Assert
        Assert.IsNotNull(loadedData);
        Assert.AreEqual(saveData.GameTime, loadedData.GameTime);
        Assert.AreEqual(saveData.CurrentScene, loadedData.CurrentScene);
        Assert.AreEqual(saveData.PlayerPosition, loadedData.PlayerPosition);
    }

    [UnityTest]
    public IEnumerator SaveSystem_InvalidSaveFileReturnsNull()
    {
        // Act
        SaveSystem.ClearSaveData();
        yield return null;

        var loadedData = SaveSystem.Load();
        
        // Assert
        Assert.IsNull(loadedData);
    }
}
