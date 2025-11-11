using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "inventoryObject", menuName = "Scriptable Objects/inventoryObject")]
public class inventoryObject : ScriptableObject
{
    [SerializeField]
    private List<inventoryItem> items;

    [field: SerializeField]
    public int Size { get; private set; } = 36;

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
            }
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
}

[Serializable]
public struct inventoryItem 
{
    public int quantity;
    public itemObject item;
    public Sprite itemImage => item != null ? item.ItemImage : null;
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
