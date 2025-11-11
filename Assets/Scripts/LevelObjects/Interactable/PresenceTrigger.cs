using UnityEngine;
using UnityEngine.Events;

namespace LevelObjects.Interactable
{
    [RequireComponent(typeof(Collider2D))]
    public class PresenceTrigger : MonoBehaviour
    {
        [Header("Trigger Layer")]
        [Tooltip("What layer can activate this trigger")]
        [SerializeField] private LayerMask triggerLayer;
        
        [Tooltip("Check if the trigger should happen continuously when object is inside")]
        [SerializeField] private readonly bool triggerContinuously = false;
        
        [Header("Trigger Events")]
        [SerializeField] private UnityEvent onTriggerEnter;
        [SerializeField] private UnityEvent onTriggerExit;
        [SerializeField] private UnityEvent onTriggerStay; //only for continuous trigger = true

        private void Reset()
        {
            Collider2D col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsInLayerMask(other.gameObject.layer))
            {
                onTriggerEnter?.Invoke();
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (IsInLayerMask(other.gameObject.layer))
            {
                onTriggerExit?.Invoke();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (triggerContinuously && IsInLayerMask(other.gameObject.layer))
            {
                onTriggerStay?.Invoke();
            }
        }

        private bool IsInLayerMask(int layer)
        {
            return (triggerLayer.value & (1 << layer)) != 0;
        }
    }
}