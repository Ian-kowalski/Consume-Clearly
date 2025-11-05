using UnityEngine;

namespace LevelObjects.Interactable
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] protected bool requiresLever = false;

        public bool RequiresLever => requiresLever;

        public abstract void Interact();
    }
}