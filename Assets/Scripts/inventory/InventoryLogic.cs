using System;
using System.Collections.Generic;
using UnityEngine;

namespace inventory
{
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

        private void HandleShowItemAction(InventoryItemLogic logic)
        {
            throw new NotImplementedException();
        }

        private void HandleItemSelected(InventoryItemLogic logic)
        {
            logic.Select();
            descriptionpanel.SetDescription(testSprite, testTitle, testDescription);
        }

        public void Show() 
        {
            gameObject.SetActive(true);
            descriptionpanel.ResetDescription();

            inventoryitems[0].SetData(testSprite, testQuantity);
        }

        public void Hide() 
        {
            gameObject.SetActive(false);
            ResetDescription();
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
}
