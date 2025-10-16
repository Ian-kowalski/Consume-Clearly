using NUnit.Framework;
using UnityEngine;

public class SaveSystemEditModeTests
{
    [Test]
    public void SaveSystem_SavesDataCorrectly()
    {
        // Arrange
        var saveData = new SaveData
        {
            GameTime = 50.0f,
            CurrentScene = "MainMenu",
            PlayerPosition = new Vector3(0f, 5f, 10f)
        };

        // Act
        SaveSystem.Save(saveData);
        var loadedData = SaveSystem.Load();

        // Assert
        Assert.IsNotNull(loadedData, "Loaded data should not be null.");
        Assert.AreEqual(saveData.GameTime, loadedData.GameTime, "GameTime is not saved correctly.");
        Assert.AreEqual(saveData.CurrentScene, loadedData.CurrentScene, "CurrentScene is not saved correctly.");
        Assert.AreEqual(saveData.PlayerPosition, loadedData.PlayerPosition, "PlayerPosition is not saved correctly.");
    }

    [Test]
    public void SaveSystem_IsSaveFileValid_WhenFileExists()
    {
        // Arrange
        var saveData = new SaveData
        {
            GameTime = 30.0f,
            CurrentScene = "TestScene",
            PlayerPosition = new Vector3(10f, 15f, 20f)
        };

        SaveSystem.Save(saveData);

        // Act
        var isValid = SaveSystem.IsSaveFileValid();

        // Assert
        Assert.IsTrue(isValid, "IsSaveFileValid should return true when a valid save file exists.");
    }

    [Test]
    public void SaveSystem_ClearSaveFile()
    {
        // Arrange
        var saveData = new SaveData
        {
            GameTime = 10.0f,
            CurrentScene = "Level1",
            PlayerPosition = Vector3.zero
        };

        SaveSystem.Save(saveData);

        // Act
        SaveSystem.ClearSaveData();
        var loadedData = SaveSystem.Load();

        // Assert
        Assert.IsNull(loadedData, "Save data should be null after clearing save file.");
    }
}
