using System;
using System.Collections.Generic;
using System;
using Inventory;
using UnityEngine;
using inventory;

public class InventoryLogic : MonoBehaviour
{
    [SerializeField]
    private InventoryItemLogic itemprefab;
    [SerializeField]
    private RectTransform contentpanel;
    [SerializeField]
    private InventoryDescription descriptionpanel;

        private List<InventoryItemLogic> inventoryitems = new List<InventoryItemLogic>();

    public event Action<int> OnDescriptionRequested, OnItemRequested;

    private void Awake()
    {
        descriptionpanel.ResetDescription();
    }

    public void InitializeInventory(int size)
    {
        inventoryitems = new List<InventoryItemLogic>();
        contentpanel.DetachChildren();
        for (int i = 0; i < size; i++)
        {
            InventoryItemLogic newitem = Instantiate(itemprefab, Vector3.zero, Quaternion.identity);
            newitem.transform.SetParent(contentpanel);
            inventoryitems.Add(newitem);
            newitem.OnItemClicked += HandleItemSelected;
            newitem.OnRightMouseBtnClick += HandleShowItemAction;
        }
    }

    public void UpdateData(int index, Sprite sprite, int quantity)
    {
        if (inventoryitems.Count >= index)
        {
            inventoryitems[index].SetData(sprite, quantity);
        }
        
    }

    private void HandleShowItemAction(InventoryItemLogic logic)
    {
        HideButtons();
        int index = inventoryitems.IndexOf(logic);
        if (index < 0) return;
        inventoryitems[index].btnEnable();
    }

    private void HandleItemSelected(InventoryItemLogic logic)
    {
        Debug.Log("Item selected");
        ResetDescription();
        int index = inventoryitems.IndexOf(logic);
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

    public void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
    {
        descriptionpanel.SetDescription(itemImage, name, description);
        DeselectAllSelections();
        inventoryitems[itemIndex].Select();
    }

    public void ResetDescription()
    {
        descriptionpanel.ResetDescription();
        DeselectAllSelections();
        HideButtons();
    }

    private void DeselectAllSelections()
    {
        foreach (InventoryItemLogic item in inventoryitems)
        {
            item.Deselect();
        }

    }

    private void HideButtons()
    {
        foreach (InventoryItemLogic item in inventoryitems)
        {
            item.btnDisable();
        }
    }

    public void inventorySizeTest()
    {
        Debug.Log("Inventory size is: " + inventoryitems.Count);
    }
}
