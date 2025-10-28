using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryLogic : MonoBehaviour
{
    [SerializeField]
    private InventoryItemLogic itemprefab;
    [SerializeField]
    private RectTransform contentpanel;
    [SerializeField]
    private InventoryDescription descriptionpanel;
    [SerializeField]
    private int inventorysize = 10;

    public Sprite testSprite;
    public int testQuantity;
    public string testTitle, testDescription;

    //public event Action<int> OnItemActionSelected, OnDescriptionRequested;

    private List<InventoryItemLogic> inventoryitems = new List<InventoryItemLogic>();

    private void Awake()
    {
        descriptionpanel.ResetDescription();
    }

    public void InitializeInventory()
    {
        inventoryitems = new List<InventoryItemLogic>();
        contentpanel.DetachChildren();
        for (int i = 0; i < inventorysize; i++)
        {
            InventoryItemLogic newitem = Instantiate(itemprefab, Vector3.zero, Quaternion.identity);
            newitem.transform.SetParent(contentpanel);
            inventoryitems.Add(newitem);
            newitem.OnItemClicked += HandleItemSelected;
            newitem.OnRightMouseBtnClick += HandleShowItemAction;
        }
    }

    //public void updateData(int itemindex, int itemQuantity, Sprite itemImage)
    //{
    //    if (inventoryitems.Count > itemindex)
    //    {
    //        inventoryitems[itemindex].SetData(itemImage, itemQuantity);
    //    }
    //}

    private void HandleShowItemAction(InventoryItemLogic logic)
    {
        throw new NotImplementedException();
    }

    private void HandleItemSelected(InventoryItemLogic logic)
    {
        logic.Select();
        descriptionpanel.SetDescription(testSprite, testTitle, testDescription);
        //int index = inventoryitems.IndexOf(logic);
        //if (index == -1) return;
        //OnDescriptionRequested?.Invoke(index);
    }

    public void Show() 
    {
        gameObject.SetActive(true);
        descriptionpanel.ResetDescription();
        //ResetDescription();

        inventoryitems[0].SetData(testSprite, testQuantity);
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
    }

    private void ResetDescription()
    {
        descriptionpanel.ResetDescription();
        DeselectAllSelections();
    }

    private void DeselectAllSelections()
    {
        foreach (InventoryItemLogic item in inventoryitems)
        {
            item.Deselect();
        }
    }
}
