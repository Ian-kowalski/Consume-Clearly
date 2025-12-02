using UnityEngine;

namespace inventory
{

    public class ItemActionPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject button;

        public void enable()
        {
            if(!button.activeSelf)
                button.SetActive(true);
            else return;
        }

        public void disable()
        {
            if (button.activeSelf)
                button.SetActive(false);
            else return;
        }
    }
}
