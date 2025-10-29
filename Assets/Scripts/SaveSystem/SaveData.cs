using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float GameTime;
    public string CurrentScene;
    public Vector3 PlayerPosition;
    
    public bool IsSceneValidForSaving()
    {
        return !string.IsNullOrEmpty(CurrentScene) && 
               CurrentScene != "Credits" && 
               CurrentScene != "MainMenu";
    }
}
