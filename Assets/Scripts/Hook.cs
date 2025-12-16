using System.Collections;
using Save;
using Inventory;
using UnityEngine;

namespace LevelObjects.Interactable
{
    public class Hook : Interactable
    {
        private InventoryController inventoryController;
        private InventoryObject _InventoryData;
        private bool _ropeAttached;
        private bool _enableRopeLogs = true;

        void RopeLog(string message)
        {
            if (_enableRopeLogs)
                Debug.Log(message);
        }

        private void Start()
        {
            inventoryController = FindFirstObjectByType<InventoryController>(FindObjectsInactive.Include);
            RopeLog("Inventory Controller found: " + (inventoryController != null));
            _InventoryData = inventoryController.InventoryObject;
            RopeLog("Inventory Data found: " + (_InventoryData != null));
        }

        public override void Interact()
        {
            StartCoroutine(UseHook());
        }

        public IEnumerator UseHook()
        {
            if (_InventoryData == null)
            {
                RopeLog("Inventory data not found.");
                yield break;
            }
            int ropeIndex = _InventoryData.FindItemIndexWithName("Rope");
            if (ropeIndex == -1)
            {
                RopeLog("Rope item not found in inventory.");
                yield break;
            }
            var ropeItem = _InventoryData.GetItemAt(ropeIndex);
            bool hasRope = !ropeItem.IsEmpty && ropeItem.Quantity > 0;
            if (hasRope && !_ropeAttached)
            {
                _InventoryData.ChangeQuantityAt(ropeIndex, 0);
                _ropeAttached = true;
                RopeLog("Rope attached to the hook.");
            }
            else if (_ropeAttached)
            {
                _InventoryData.ChangeQuantityAt(ropeIndex, 1);
                _ropeAttached = false;
                RopeLog("Rope removed from the hook.");
            }
            else
            {
                RopeLog("No rope available in inventory or to take from hook.");
                RopeLog("has rope= " + hasRope + " ropeattached= " + _ropeAttached);
            }
            yield break;
        }
        

        public override InteractableObjectState SaveState()
        {
            return new InteractableObjectState
            {
                uniqueId = GetUniqueId(),
                isActive = _ropeAttached,
            };
        }

        public override void LoadState(InteractableObjectState state)
        {
            if (state == null || state.uniqueId != GetUniqueId()) return;
            StopAllCoroutines();

            _ropeAttached = state.isActive;
            RopeLog("Loaded ropeAttached (from isActive): " + _ropeAttached);
        }
    }
}

