using System.Collections;
using LevelObjects.Interactable;
using Save;
using UnityEngine;

public class rock : Interactable
{
    private Collider2D collider;
    private SpriteRenderer sprite;

    [SerializeField] private Animator poofAnimator;
    [SerializeField] private string poofTrigger = "Poof";

    void Start()
    {
        collider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public override void Interact()
    {
        if (collider != null) collider.enabled = false;

        if (poofAnimator != null)
        {
            poofAnimator.SetTrigger(poofTrigger);
            StartCoroutine(DisableAfterPoof());
        }
        else
        {
            if (sprite != null) sprite.enabled = false;
        }
    }

    private IEnumerator DisableAfterPoof()
    {
        float duration = 0f;

        var controller = poofAnimator?.runtimeAnimatorController;
        if (controller != null && controller.animationClips != null && controller.animationClips.Length > 0)
        {
            duration = controller.animationClips[0].length;
        }

        if (duration > 0f)
            yield return new WaitForSeconds(duration);
        else
            yield return null; // wait one frame so the trigger can take effect

        if (sprite != null) sprite.enabled = false;
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
