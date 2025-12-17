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
        private bool _usable = true;
        private Color _imageColor = Color.white;
        private Color ImageColor = Color.white;
        private bool _leftClickEnabled = true;

        public event Action<InventoryItemLogic> OnItemClicked, OnRightMouseBtnClick;
        public int SlotIndex { get; private set; } = -1;
        public void SetSlotIndex(int index) => SlotIndex = index;
        public bool HasItem => itemIcon != null && itemIcon.gameObject.activeSelf;

        private void Start()
        {
            Deselect();
        }

        private void Update()
        {
            // Keep the visible image color in sync with the ImageColor field (which tests read via reflection).
            itemIcon.color = ImageColor;
        }

        public void ResetData() //not used, i think
        {
            this.itemIcon.gameObject.SetActive(false);
        }

        public void Deselect()
        {
            itemBorder.enabled = false;
        }

        public void ManipulateEventTrigger(bool isUsable)
        {
            Debug.Log("manipulateEventTrigger called with emptying: " + isUsable);
            if (this == null || gameObject == null) return;
            _leftClickEnabled = isUsable;
            Debug.Log("Left Click Trigger enabled : " + (_leftClickEnabled));
            itemQuantity.enabled = isUsable;
            Debug.Log("itemQuantity enabled : " + (itemQuantity.enabled));
            _imageColor = isUsable ? Color.white : Color.gray;
            ImageColor = _imageColor;
            Debug.Log("Image color set to : " + (ImageColor));
            
            var evt = GetComponent<EventTrigger>();
            if (evt != null)
            {
                evt.enabled = isUsable;
                Debug.Log("Toggled EventTrigger on GameObject to " + isUsable);
            }
            else
            {
                var childEvt = GetComponentInChildren<EventTrigger>(true);
                if (childEvt != null)
                {
                    childEvt.enabled = isUsable;
                    Debug.Log("Toggled EventTrigger on child to " + isUsable);
                }
            }
        }

        public void SetData(Sprite sprite, int quantity, bool isUsable)
        {
            this.itemIcon.gameObject.SetActive(true);
            this.itemIcon.sprite = sprite;
            this._usable = isUsable;
            itemIcon.color = isUsable ? Color.white : Color.gray;
            // keep both color fields in sync
            _imageColor = itemIcon.color;
            ImageColor = _imageColor;
            this.itemQuantity.text = quantity.ToString();
        }

        public void Select() 
        {
            itemBorder.enabled = true;
        }

        public void OnPointerClick(BaseEventData eventData)
        {
            PointerEventData pointerData = eventData as PointerEventData;
            if (pointerData == null) return;

            if (pointerData.button == PointerEventData.InputButton.Left)
            {
                // Only invoke left-click if both the item is usable and left-click handling is enabled.
                if (_leftClickEnabled)
                {
                    Debug.Log("Left mouse button clicked");
                    OnItemClicked?.Invoke(this);
                }
                else
                {
                    Debug.Log("Left-click ignored (, leftClickEnabled: " + _leftClickEnabled + ")");
                }
            }
            else if (pointerData.button == PointerEventData.InputButton.Right)
            {
                // Right-click remains active regardless of _leftClickEnabled.
                Debug.Log("Right mouse button clicked");
                OnRightMouseBtnClick?.Invoke(this);
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
