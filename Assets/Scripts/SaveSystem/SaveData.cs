using System;
using UnityEngine;

namespace SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public float GameTime;
        public string CurrentScene;
        public Vector3 PlayerPosition;
    }
}
