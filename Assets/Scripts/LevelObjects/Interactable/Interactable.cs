using UnityEngine;
using Save;

namespace LevelObjects.Interactable
{
    public abstract class Interactable : MonoBehaviour, ISaveable
    {
        [SerializeField] protected bool requiresLever = false;

        [Header("Save System")] [SerializeField]
        private string uniqueId;

        public bool RequiresLever => requiresLever;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                uniqueId = System.Guid.NewGuid().ToString();
            }
        }

        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                uniqueId = System.Guid.NewGuid().ToString();
            }
        }

        public string GetUniqueId()
        {
            return uniqueId;
        }

        public abstract void Interact();
        public abstract InteractableObjectState SaveState();
        public abstract void LoadState(InteractableObjectState state);
    }
}