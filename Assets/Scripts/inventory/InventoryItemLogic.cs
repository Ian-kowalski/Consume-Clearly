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
            PointerEventData pointerData = eventData as PointerEventData;
            if (pointerData != null)
            {
                if (empty)
                {
                    return;
                }
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
}
