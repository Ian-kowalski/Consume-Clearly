using UnityEngine;
using Save;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Player;

namespace LevelObjects.Interactable
{

    [RequireComponent(typeof(Collider2D))]
    public class InteractableScene : Interactable
    {
        [Header("Scene to load")] [Tooltip("Choose which scene needs to be load")] [SerializeField]
        private Object sceneToLoad;

        public override void Interact()
        {
            var scene=sceneToLoad.name;
            Debug.Log(scene);
            SceneManager.LoadScene(scene);
        }

        public override InteractableObjectState SaveState()
        {
            return new InteractableObjectState
            {
                uniqueId = GetUniqueId(),
                position = transform.position,
            };
        }

        public override void LoadState(InteractableObjectState state)
        {
            if (state == null || state.uniqueId != GetUniqueId()) return;

            Debug.Log("Loading Scene Changer");
        }
    }
}
