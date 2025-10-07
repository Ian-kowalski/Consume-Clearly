using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.Overlays;

public static class SaveSystem
{
    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "save.save");

    public static void Save(SaveData saveData)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(SaveFilePath, FileMode.Create))
            {
                formatter.Serialize(stream, saveData);
            }

            Debug.Log($"Game saved successfully at {SaveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    public static SaveData Load()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(SaveFilePath, FileMode.Open))
                {
                    return formatter.Deserialize(stream) as SaveData;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading save file: {e.Message}");
                return null;
            }
        }

        return null;
    }

    public static void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("Save file deleted");
        }
    }
}