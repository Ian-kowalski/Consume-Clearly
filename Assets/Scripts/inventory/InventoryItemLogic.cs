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
        private Color ImageColor = Color.white;
        private bool triggerEnabled = true;
        private EventTrigger trigger;


        public event Action<InventoryItemLogic> OnItemClicked, OnRightMouseBtnClick;

        private void Start()
        {
            trigger = this.GetComponent<EventTrigger>();
            Deselect();
        }

        private void Update()
        {
            itemIcon.color = ImageColor;
            trigger.enabled = triggerEnabled;
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
            trigger = this.GetComponent<EventTrigger>();
            Debug.Log("" + trigger);
            if (trigger != null)
            {
                triggerEnabled = emptying;
                Debug.Log("EventTrigger enabled : " + (triggerEnabled));
                itemQuantity.enabled = emptying;
                Debug.Log("itemQuantity enabled : " + (itemQuantity.enabled));
                ImageColor = emptying ? Color.white : Color.gray;
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
