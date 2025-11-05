using UnityEngine;

namespace LevelObjects.Interactable
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class LiftGround : MonoBehaviour
    {
        private LiftPlatform parentLift;

        private void Start()
        {
            parentLift = GetComponentInParent<LiftPlatform>();
            if (parentLift == null)
            {
                Debug.LogError("LiftGround must be a child of an object with LiftPlatform script!");
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (parentLift != null)
            {
                parentLift.OnPlayerEntered(collision.gameObject);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (parentLift != null)
            {
                parentLift.OnPlayerExited(collision.gameObject);
            }
        }
    }
}