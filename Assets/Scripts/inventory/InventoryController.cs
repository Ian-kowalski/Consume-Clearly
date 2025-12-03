using Items;
using UnityEngine;

namespace inventory
{
    public class InventoryController : MonoBehaviour
    {
        private InventoryLogic inventory;
        [SerializeField]
        private inventoryObject inventoryData;
        public inventoryObject InventoryObject => inventoryData;

        private void Start()
        {
            PrepareUI();
            inventory.inventorySizeTest();
        }

        private void PrepareUI()
        {
            inventory = FindFirstObjectByType<InventoryLogic>(FindObjectsInactive.Include);
            inventory.InitializeInventory(inventoryData.Size);
            inventory.OnDescriptionRequested += HandleDescriptionRequest;
            InventoryObject.OnItemModified += inventory.manipulateItemFuction;
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            Debug.Log($"in handledescriptionrequest");
            inventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.isEmpty)
            {
                inventory.ResetDescription();
                return;
            }
            itemObject item = inventoryItem.item;
            inventory.UpdateDescription(itemIndex, item.ItemImage,
                item.name, item.Description);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!inventory.isActiveAndEnabled)
                {
                    inventory.Show();
                    foreach (var item in inventoryData.GetCurrentInventoryState())
                    {
                        inventory.UpdateData(item.Key,
                            item.Value.item.ItemImage,
                            item.Value.quantity,
                            item.Value.item.IsUsable);
                    }
                }
                else
                {
                    inventory.Hide();
                }
            }
        }
    }
}
