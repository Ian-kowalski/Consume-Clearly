using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float GameTime;
    public string CurrentScene;
    public Vector3 PlayerPosition;
    
    private static readonly HashSet<string> InvalidScenes = new() { "Credits", "MainMenu" };
    
    public bool IsSceneValidForSaving()
    {
        return !string.IsNullOrEmpty(CurrentScene) && InvalidScenes.Contains(CurrentScene) == false;
    }
}
