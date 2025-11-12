using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace inventory
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
        private ItemActionPanel itemactionpanel;

        public event Action<InventoryItemLogic> OnItemClicked, OnRightMouseBtnClick;

        private bool empty = true;

        private void Start()
        {
            Deselect();
        }

        public void ResetData()
        {
            this.itemIcon.gameObject.SetActive(false);
            empty = true;
        }

        public void Deselect()
        {
            itemBorder.enabled = false;
        }

        public void SetData(Sprite sprite, int quantity)
        {
            this.itemIcon.gameObject.SetActive(true);
            this.itemIcon.sprite = sprite;
            this.itemQuantity.text = quantity.ToString();
            empty = false;
        }

        public void Select() 
        {
            itemBorder.enabled = true;
        }

        public void OnPointerClick(BaseEventData eventData)
        {
            Debug.Log("Pointer click detected on inventory item.");
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

        public void btnEnable()
        {
            itemactionpanel.enable();
        }

        public void btnDisable()
        {
            itemactionpanel.disable();
        }

    }
}
