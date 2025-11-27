using Save;
using UnityEngine;

namespace LevelObjects.Interactable
{
    public class rock : Interactable
    {
        private Collider2D collider;
        private SpriteRenderer sprite;

        void Start()
        {
            collider = GetComponent<Collider2D>();
            sprite = GetComponent<SpriteRenderer>();
        }

        public override void Interact()
        {
            collider.enabled = false;
            sprite.enabled = false;
        }

        public override InteractableObjectState SaveState()
        {
            return new InteractableObjectState
            {
                uniqueId = gameObject.name,
                isActive = sprite != null && sprite.enabled && collider != null && collider.enabled,
                position = transform.position,
                rotation = transform.rotation
            };
        }

        public override void LoadState(InteractableObjectState state)
        {
            if (state == null) return;

            transform.position = state.position;
            transform.rotation = state.rotation;

            bool active = state.isActive;
            if (sprite != null) sprite.enabled = active;
            if (collider != null) collider.enabled = active;
        }
    }
}