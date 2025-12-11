using Codice.Client.BaseCommands;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

        public void ManipulateItem(int index, int newQuantity, int prevQuantity)
        {
            Debug.Log("Manipulating item at index: " + index);
            ItemObject item = _items[index].Item;
            if (item != null)
            {
                Debug.Log("Item found: " + item.name);
                Debug.Log("Previous Quantity: " + prevQuantity + ", New Quantity: " + newQuantity);
                bool crossedThreshold =
                    (prevQuantity == 0 && newQuantity > 0) || // 0 to 1 and more
                    (prevQuantity > 0 && newQuantity == 0);   // 1 and more to 0
                Debug.Log("Crossed threshold: " + crossedThreshold);

                if (crossedThreshold)
                {
                    Debug.Log("item usability is:" + item.IsUsable);
                    item.IsUsable = !item.IsUsable;
                    bool test = item.IsUsable;
                    Debug.Log("Item usability changed to: " + item.IsUsable);
                    Debug.Log("test is" + test);

                    _items[index].Item.IsUsable = item.IsUsable;
                    OnItemModified?.Invoke(index, test);
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
