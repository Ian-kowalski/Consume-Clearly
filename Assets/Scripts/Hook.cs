using System.Collections;
using Save;
using inventory;
using UnityEngine;

namespace LevelObjects.Interactable
{
    public class Hook : Interactable
    {
        private InventoryController inventoryController;
        private inventoryObject InventoryData;
        private bool RopeAttached;
        private bool EnableRopeLogs = true;

        void RopeLog(string message)
        {
            if (EnableRopeLogs)
                Debug.Log(message);
        }

        private void Start()
        {
            inventoryController = FindFirstObjectByType<InventoryController>(FindObjectsInactive.Include);
            RopeLog("Inventory Controller found: " + (inventoryController != null));
            InventoryData = inventoryController.InventoryObject;
            RopeLog("Inventory Data found: " + (InventoryData != null));
        }

        public override void Interact()
        {
            StartCoroutine(UseHook());
        }

        public IEnumerator UseHook()
        {
            if (InventoryData == null)
            {
                RopeLog("Inventory data not found.");
                yield break;
            }
            int ropeIndex = InventoryData.FindItemIndexWithName("Rope");
            if (ropeIndex == -1)
            {
                RopeLog("Rope item not found in inventory.");
                yield break;
            }
            var ropeItem = InventoryData.GetItemAt(ropeIndex);
            bool hasRope = !ropeItem.isEmpty && ropeItem.quantity > 0;
            if (hasRope && !RopeAttached)
            {
                InventoryData.ChangeQuantityAt(ropeIndex, 0);
                RopeAttached = true;
                RopeLog("Rope attached to the hook.");
            }
            else if (RopeAttached)
            {
                InventoryData.ChangeQuantityAt(ropeIndex, 1);
                RopeAttached = false;
                RopeLog("Rope removed from the hook.");
            }
            else
            {
                RopeLog("No rope available in inventory or to take from hook.");
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

