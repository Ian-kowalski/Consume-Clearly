using System.IO;
using UnityEngine;

namespace SaveSystem
{
    public static class SaveSystem
    {
        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "SaveData.json");

        public static void Save(SaveData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(SaveFilePath, json);
                Debug.Log("Game saved successfully!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save game: {ex.Message}");
            }
        }

        public static SaveData Load()
        {
            if (File.Exists(SaveFilePath))
            {
                try
                {
                    string json = File.ReadAllText(SaveFilePath);
                    SaveData data = JsonUtility.FromJson<SaveData>(json);
                    Debug.Log("Game loaded successfully!");
                    return data;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to load game: {ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning("Save file not found.");
            }

            return null; // Return null if loading fails or file doesn't exist.
        }
    
        public static bool IsSaveFileValid()
        {
            SaveData saveData = Load();
            return saveData != null && !string.IsNullOrEmpty(saveData.CurrentScene);
        }
    
        public static void ClearSaveData()
        {
            var saveFilePath = $"{Application.persistentDataPath}/SaveData.json"; // Adjust if needed.
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log("Save data cleared.");
            }
        }
    }
}
