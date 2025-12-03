using Codice.Client.BaseCommands;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace inventory
{
    [CreateAssetMenu(fileName = "inventoryObject", menuName = "Scriptable Objects/inventoryObject")]
    public class inventoryObject : ScriptableObject
    {
        [SerializeField]
        private List<inventoryItem> items;

        [field: SerializeField]
        public int Size { get; private set; } = 36;

        public event Action<int, bool> OnItemModified;

        public void Initialize()
        {
            items = new List<inventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                items.Add(inventoryItem.getEmpty());
            }

        }

        public void AddItem(itemObject item, int quantity)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].isEmpty)
                {
                    items[i] = new inventoryItem { item = item, quantity = quantity };
                    return;
                }
            }
        }

        public void manipulateItem(int Index, int NewQuantity, int PrevQuantity)
        {
            Debug.Log("Manipulating item at index: " + Index);
            itemObject item = items[Index].item;
            if (item != null)
            {
                Debug.Log("Item found: " + item.name);
                Debug.Log("Previous Quantity: " + PrevQuantity + ", New Quantity: " + NewQuantity);
                bool crossedThreshold =
                    (PrevQuantity == 0 && NewQuantity > 0) || // 0 to 1 and more
                    (PrevQuantity > 0 && NewQuantity == 0);   // 1 and more to 0
                Debug.Log("Crossed threshold: " + crossedThreshold);

                if (crossedThreshold)
                {
                    Debug.Log("item usability is:" + item.IsUsable);
                    item.IsUsable = !item.IsUsable;
                    bool test = item.IsUsable;
                    Debug.Log("Item usability changed to: " + item.IsUsable);
                    Debug.Log("test is" + test);

                    items[Index].item.IsUsable = item.IsUsable;
                    OnItemModified?.Invoke(Index, test);
                }
            }
            else Debug.Log("No item to at index: " + Index);
        }

        public void ChangeQuantityAt(int index, int newAmount)
        {
            if (index < 0 || index >= items.Count)
            {
                Debug.LogWarning($"ChangeQuantityAt: index out of range {index}");
                return;
            }
            var current = items[index];
            if (current.isEmpty)
            {
                Debug.LogWarning($"ChangeQuantityAt: no item at index {index}");
                return;
            }
            int prevQuantity = current.quantity;
            Debug.Log("Previous quantity: " + prevQuantity);
            Debug.Log("newAmount: " + newAmount);
            if (current.item != null)
            {
                manipulateItem(index, newAmount, prevQuantity);
                items[index] = current.ChangeQuantity(newAmount);
            }
        }


        public Dictionary<int, inventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, inventoryItem> itemDictionary = new Dictionary<int, inventoryItem>();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].isEmpty) continue;
                itemDictionary[i] = items[i];
            }
            return itemDictionary;
        }

        public inventoryItem GetItemAt(int itemIndex)
        {
            return items[itemIndex];
        }

        public int FindItemIndexWithName(String Name)
        {
            int index = items.FindIndex(item => !item.isEmpty && item.itemName == Name);
            return index;
        }
    }

    [Serializable]
    public struct inventoryItem 
    {
        public int quantity;
        public itemObject item;
        public Sprite itemSprite => item != null ? item.ItemImage : null;
        public string itemDescription => item != null ? item.Description : "";
        public string itemName => item != null ? item.name : "";
        public bool isEmpty => item == null;

        public inventoryItem ChangeQuantity(int newAmount)
        {
            return new inventoryItem
            {
                item = this.item,
                quantity = newAmount
            };
        }

        public static inventoryItem getEmpty() => new inventoryItem { item = null, quantity = 0 };
    }
}
