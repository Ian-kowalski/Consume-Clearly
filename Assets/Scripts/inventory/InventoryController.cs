using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private InventoryLogic inventory;

    private void Start()
    {
        inventory = FindFirstObjectByType<InventoryLogic>(FindObjectsInactive.Include);
        inventory.InitializeInventory();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory.isActiveAndEnabled == false)
            {
                Debug.Log("Showing Inventory");
                inventory.Show();
                Debug.Log("Inventory shown" + inventory.gameObject.activeSelf);
            }
            else
            {
                inventory.Hide();
            }
        }
    }
}
