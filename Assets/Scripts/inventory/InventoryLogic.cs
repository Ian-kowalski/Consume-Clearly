using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryLogic : MonoBehaviour
    {
        [SerializeField]
        private InventoryItemLogic _itemPrefab;
        [SerializeField]
        private RectTransform _contentPanel;
        [SerializeField]
        private InventoryDescription _descriptionPanel;

        private List<InventoryItemLogic> _inventoryItems = new List<InventoryItemLogic>();

        public event Action<int> OnDescriptionRequested;

        private void Awake()
        {
            _descriptionPanel.ResetDescription();
        }

        public void InitializeInventory(int size)
        {
            _inventoryItems = new List<InventoryItemLogic>();
            _contentPanel.DetachChildren();
            for (int i = 0; i < size; i++)
            {
                InventoryItemLogic newitem = Instantiate(_itemPrefab, Vector3.zero, Quaternion.identity);
                newitem.transform.SetParent(_contentPanel);
                _inventoryItems.Add(newitem);
                newitem.OnItemClicked += HandleItemSelected;
                newitem.OnRightMouseBtnClick += HandleShowItemAction;
            }
        }

        public void UpdateData(int index, Sprite sprite, int quantity, bool isUsable)
        {
            if (index < 0 || index >= _inventoryItems.Count) return;
            _inventoryItems[index].SetData(sprite, quantity,isUsable);
        }

        public void ManipulateItemFuction(int index, bool emptying)
        {
            Debug.Log("Manipulating item function called for index: " + index + " with emptying: " + emptying);
            _inventoryItems[index].ManipulateEventTrigger(emptying);
        }

        private void HandleShowItemAction(InventoryItemLogic logic)
        {
            //HideButtons();
            //int index = inventoryitems.IndexOf(logic);
            //if (index < 0) return;
            //inventoryitems[index].btnEnable();
        }

        private void HandleItemSelected(InventoryItemLogic logic)
        {
            Debug.Log("Item selected");
            ResetDescription();
            int index = _inventoryItems.IndexOf(logic);
            if (index < 0) return;
            OnDescriptionRequested?.Invoke(index);
        }

        public void Show() 
        {
            gameObject.SetActive(true);
            ResetDescription();
        }

        public void Hide() 
        {
            gameObject.SetActive(false);
            ResetDescription();
        }

        public void UpdateDescription(int itemIndex, Sprite itemSprite, string name, string description)
        {
            _descriptionPanel.SetDescription(itemSprite, name, description);
            DeselectAllSelections();
            _inventoryItems[itemIndex].Select();
        }

        public void ResetDescription()
        {
            _descriptionPanel.ResetDescription();
            DeselectAllSelections();
            HideButtons();
        }

        private void DeselectAllSelections()
        {
            foreach (InventoryItemLogic item in _inventoryItems)
            {
                item.Deselect();
            }

        }

        private void HideButtons()
        {
            foreach (InventoryItemLogic item in _inventoryItems)
            {
                item.ButtonDisable();
            }
        }

        public void InventorySizeTest()
        {
            Debug.Log("Inventory size is: " + _inventoryItems.Count);
        }
    }
}
