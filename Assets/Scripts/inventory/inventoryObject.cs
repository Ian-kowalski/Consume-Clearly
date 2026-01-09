using System;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "inventoryObject", menuName = "Scriptable Objects/inventoryObject")]
    public class InventoryObject : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItem> _items;

        [field: SerializeField]
        public int Size { get; private set; } = 36;

        public event Action<int, bool> OnItemModified;

        public void Initialize()
        {
            _items = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                _items.Add(InventoryItem.GetEmpty());
            }

        }

        public void AddItem(ItemObject item, int quantity)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].IsEmpty)
                {
                    _items[i] = new InventoryItem { Item = item, Quantity = quantity };
                    return;
                }
            }
        }

        public void RemoveItem(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                Debug.LogWarning($"removeItem: index out of range {index}");
                return;
            }
            _items[index] = InventoryItem.GetEmpty();
        }

        public void ManipulateItem(int index, int newQuantity, int prevQuantity)
        {
            Debug.Log("Manipulating item at index: " + index);
            ItemObject item = _items[index].Item;
            if (item != null)
            {
                bool wasEmpty = prevQuantity == 0;
                bool isEmptyNow = newQuantity == 0;
                bool crossedThreshold = wasEmpty != isEmptyNow;
                if (crossedThreshold)
                {
                    item.IsUsable = newQuantity > 0;
                    _items[index].Item.IsUsable = item.IsUsable;
                    OnItemModified?.Invoke(index, item.IsUsable);
                }
            }
            else Debug.Log("No item to at index: " + index);
        }

        public void ChangeQuantityAt(int index, int newAmount)
        {
            if (index < 0 || index >= _items.Count)
            {
                Debug.LogWarning($"ChangeQuantityAt: index out of range {index}");
                return;
            }
            var current = _items[index];
            if (current.IsEmpty)
            {
                Debug.LogWarning($"ChangeQuantityAt: no item at index {index}");
                return;
            }
            int prevQuantity = current.Quantity;
            Debug.Log("Previous quantity: " + prevQuantity);
            Debug.Log("newAmount: " + newAmount);
            if (current.Item != null)
            {
                if (current.Item.MaxStackSize < newAmount)
                {
                    Debug.LogWarning($"ChangeQuantityAt: new amount {newAmount} exceeds max stack size {current.Item.MaxStackSize} for item {current.Item.name}");
                    return;
                }
                ManipulateItem(index, newAmount, prevQuantity);
                _items[index] = current.ChangeQuantity(newAmount);
            }
        }


        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> itemDictionary = new Dictionary<int, InventoryItem>();
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].IsEmpty) continue;
                itemDictionary[i] = _items[i];
            }
            return itemDictionary;
        }

        public InventoryItem GetItemAt(int itemIndex)
        {
            return _items[itemIndex];
        }

        public int FindItemIndexWithName(String name)
        {
            int index = _items.FindIndex(item => !item.IsEmpty && item.ItemName == name);
            return index;
        }
    }

    [Serializable]
    public struct InventoryItem 
    {
        public int Quantity;
        public ItemObject Item;
        public Sprite ItemSprite => Item != null ? Item.ItemImage : null;
        public string ItemDescription => Item != null ? Item.Description : "";
        public string ItemName => Item != null ? Item.name : "";
        public bool IsEmpty => Item == null;

        public InventoryItem ChangeQuantity(int newAmount)
        {
            return new InventoryItem
            {
                Item = this.Item,
                Quantity = newAmount
            };
        }

        public static InventoryItem GetEmpty() => new InventoryItem { Item = null, Quantity = 0 };
    }
}
