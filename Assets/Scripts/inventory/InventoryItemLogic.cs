using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryItemLogic : MonoBehaviour
    {
        [Header("Unity objects")]
        [SerializeField]
        private Image itemIcon;
        [SerializeField]
        private TMP_Text itemQuantity;
        [SerializeField]
        private Image itemBorder;
        [SerializeField]
        private ItemActionPanel _itemActionPanel;
        [SerializeField]
        private bool _usable;
        private Color ImageColor;


        public event Action<InventoryItemLogic> OnItemClicked, OnRightMouseBtnClick;

        private void Start()
        {
            Deselect();
        }

        private void Update()
        {
            itemIcon.color = ImageColor;
        }

        public void ResetData()
        {
            this.itemIcon.gameObject.SetActive(false);
        }

        public void Deselect()
        {
            itemBorder.enabled = false;
        }

        public void ManipulateEventTrigger(bool emptying)
        {
            Debug.Log("manipulateEventTrigger called with emptying: " + emptying);
            if (this == null || gameObject == null) return; // the object was destroyed
            EventTrigger trigger = this.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                trigger.enabled = emptying;
                Debug.Log("EventTrigger enabled : " + (trigger.enabled));
                itemQuantity.enabled = emptying;
                Debug.Log("itemQuantity enabled : " + (itemQuantity.enabled));
                ImageColor = emptying ? Color.white : Color.gray;
                // Apply color immediately to the visible icon to avoid depending on Update() timing in tests/runtime
                if (itemIcon != null)
                {
                    itemIcon.color = ImageColor;
                }
            }
        }

        public void SetData(Sprite sprite, int quantity, bool isUsable)
        {
            this.itemIcon.gameObject.SetActive(true);
            this.itemIcon.sprite = sprite;
            this._usable = isUsable;
            itemIcon.color = isUsable ? Color.white : Color.gray;
            this.itemQuantity.text = quantity.ToString();
        }

        public void Select() 
        {
            itemBorder.enabled = true;
        }

        public void OnPointerClick(BaseEventData eventData)
        {
            if (_usable)
            {
                PointerEventData pointerData = eventData as PointerEventData;
                if (pointerData != null)
                {
                    if (pointerData.button == PointerEventData.InputButton.Left)
                    {
                        Debug.Log("Left click detected on inventory item.");
                        OnItemClicked?.Invoke(this);
                    }
                    else if (pointerData.button == PointerEventData.InputButton.Right)
                    {
                        OnRightMouseBtnClick?.Invoke(this);
                    }
                }
            }
        }

        public void ButtonEnable()
        {
            _itemActionPanel.Enable();
        }

        public void ButtonDisable()
        {
            _itemActionPanel.Disable();
        }

    }
}
