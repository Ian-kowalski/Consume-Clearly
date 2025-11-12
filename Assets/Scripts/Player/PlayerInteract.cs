using UnityEngine;
using LevelObjects.Interactable;

namespace Player
{
    public class PlayerInteract : MonoBehaviour
    {
        public float interactRange = 1.5f;
        public KeyCode interactKey = KeyCode.Q;
        public LayerMask interactableLayer;

        void Update()
        {
            if (Input.GetKeyDown(interactKey))
            {
                // Raycast to find an interactable object in front of the player
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, interactRange,
                    interactableLayer);

                if (hit.collider != null)
                {
                    Interactable interactable = hit.collider.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        // Only interact if the object doesn't require a lever
                        if (!interactable.RequiresLever)
                        {
                            interactable.Interact();
                        }
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * interactRange);
        }
    }
}