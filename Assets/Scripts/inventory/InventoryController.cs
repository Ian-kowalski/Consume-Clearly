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
                inventory.Show();
            }
            else
            {
                inventory.Hide();
            }
        }
    }
}
