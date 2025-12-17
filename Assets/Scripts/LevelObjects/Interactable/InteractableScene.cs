using UnityEngine;
using Save;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using Player;

namespace LevelObjects.Interactable
{

    [RequireComponent(typeof(Collider2D))]
    public class InteractableScene : Interactable
    {
        [Header("Scene to load")] [Tooltip("Choose which scene needs to be load")] [SerializeField]
        private Object sceneToLoad;
        
        private AnimationController animationController;
        private IEnumerator coroutine;

        private void Start()
        {
            animationController=GameObject.FindGameObjectWithTag("Player").GetComponent<AnimationController>();
            animationController.SetTurnBack(false);
        }

        public override void Interact()
        {
        
            PlayerSpawnPoint[] spawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
            if (spawnPoints.Length > 0)
            {
                Debug.Log("More than one spawn point");
                foreach (var spawnPoint in spawnPoints)
                {
                    if (spawnPoint.transform == transform)
                    {
                        spawnPoint.SetDefault();
                    }
                    else
                    {
                        spawnPoint.SetCheckpoint();
                    }
                }
            }
            animationController.SetTurnBack(true);
            coroutine=WaitAnimFinish(1.0f);
            StartCoroutine(coroutine);
        }

        private IEnumerator WaitAnimFinish(float waitTime)
        {
            var scene=sceneToLoad.name;
            yield return new WaitForSeconds(waitTime);
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
            Debug.Log("load interactive Scene");
        }
    }
}
