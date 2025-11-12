using System;
using UnityEngine;

namespace Save
{
    [Serializable]
    public class InteractableObjectState
    {
        public string uniqueId;
        public bool isActive;
        public Vector3 position;
        public Quaternion rotation;
    }
}
