using System.Collections;
using Save;
using inventory;
using UnityEngine;

namespace LevelObjects.Interactable
{
    public class Hook : Interactable
    {
        private InventoryController inventoryC;
        private inventoryObject InventoryData;
        private bool RopeAttached;

        private void Start()
        {
            inventoryC = FindFirstObjectByType<InventoryController>(FindObjectsInactive.Include);
            Debug.Log("Inventory Controller found: " + (inventoryC != null));
            InventoryData = inventoryC.InventoryObject;
            Debug.Log("Inventory Data found: " + (InventoryData != null));
        }

        public override void Interact()
        {
            StartCoroutine(UseHook());
        }

        public IEnumerator UseHook()
        {
            if (InventoryData == null)
            {
                Debug.Log("Inventory data not found.");
                yield break;
            }
            int ropeIndex = InventoryData.FindItemIndexWithName("Rope");
            if (ropeIndex == -1)
            {
                Debug.Log("Rope item not found in inventory.");
                yield break;
            }
            var ropeItem = InventoryData.GetItemAt(ropeIndex);
            bool hasRope = !ropeItem.isEmpty && ropeItem.quantity > 0;
            if (hasRope && !RopeAttached)
            {
                InventoryData.ChangeQuantityAt(ropeIndex, 0);
                RopeAttached = true;
                Debug.Log("Rope attached to the hook.");
            }
            else if (RopeAttached)
            {
                InventoryData.ChangeQuantityAt(ropeIndex, 1);
                RopeAttached = false;
                Debug.Log("Rope removed from the hook.");
            }
            else
            {
                Debug.Log("No rope available in inventory or to take from hook.");
            }

            yield break;
        }
        

        public override InteractableObjectState SaveState()
        {
            return new InteractableObjectState
            {
                uniqueId = GetUniqueId(),
                isActive = gameObject.activeSelf
            };
        }

        public override void LoadState(InteractableObjectState state)
        {
            if (state == null || state.uniqueId != GetUniqueId()) return;
            StopAllCoroutines();
        }
    }
}

