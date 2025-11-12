using LevelObjects.Interactable;
using Save;
using UnityEngine;

public class rock : Interactable
{
    private Collider2D collider;

    private SpriteRenderer sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        throw new System.NotImplementedException();
    }

    public override void LoadState(InteractableObjectState state)
    {
        throw new System.NotImplementedException();
    }
}
