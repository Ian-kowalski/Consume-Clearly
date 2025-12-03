using UnityEngine;

namespace Inventory
{

    public class ItemActionPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject button;

        public void Enable()
        {
            if(!button.activeSelf)
                button.SetActive(true);
            else return;
        }

        public void Disable()
        {
            if (button.activeSelf)
                button.SetActive(false);
            else return;
        }
    }
}
