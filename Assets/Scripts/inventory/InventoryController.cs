using Items;
using UnityEngine;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        private InventoryLogic _inventoryLogic;
        [SerializeField]
        private InventoryObject _inventoryData;
        public InventoryObject InventoryObject => _inventoryData;
        private void Start()
        {
            PrepareUI();
            _inventoryLogic.InventorySizeTest();
        }

        private void PrepareUI()
        {
            _inventoryLogic = FindFirstObjectByType<InventoryLogic>(FindObjectsInactive.Include);
            _inventoryLogic.InitializeInventory(_inventoryData.Size);
            _inventoryLogic.OnDescriptionRequested += HandleDescriptionRequest;
            InventoryObject.OnItemModified += _inventoryLogic.ManipulateItemFuction;
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = _inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                _inventoryLogic.ResetDescription();
                return;
            }
            ItemObject item = inventoryItem.Item;
            _inventoryLogic.UpdateDescription(itemIndex, item.ItemImage,
                item.name, item.Description);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!_inventoryLogic.isActiveAndEnabled)
                {
                    _inventoryLogic.Show();
                    foreach (var item in _inventoryData.GetCurrentInventoryState())
                    {
                        _inventoryLogic.UpdateData(item.Key,
                            item.Value.Item.ItemImage,
                            item.Value.Quantity,
                            item.Value.Item.IsUsable);
                    }
                }
                else
                {
                    _inventoryLogic.Hide();
                }
            }
        }
    }
}
