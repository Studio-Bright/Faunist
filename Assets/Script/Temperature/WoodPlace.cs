using UnityEngine;
using static PickupItem;

public class WoodPlace : MonoBehaviour
{
    [SerializeField] private TemperatureManager temperatureManager;
    [SerializeField] private InventorySystem inventory;

    public void PutIntoFire()
    {
        if (inventory == null)
        {
            Debug.LogError("WoodPlace: InventorySystem is not assigned!");
            return;
        }

        if (temperatureManager == null)
        {
            Debug.LogError("WoodPlace: TemperatureManager is not assigned!");
            return;
        }

        // Get the item currently selected in the player's inventory
        PickupItem selectedItem = inventory.GetSelectedItem();

        if (selectedItem == null)
        {
            Debug.Log("You need to select some wood first.");
            return;
        }

        // Check if selected item is actually wood
        if (selectedItem.woodType == WoodType.NotWood)
        {
            Debug.Log("This item is not wood.");
            return;
        }

        // RED WOOD
        if (selectedItem.woodType == WoodType.Red)
        {
            temperatureManager.AddRedWood();

            ConsumeWood(selectedItem);

            Debug.Log("Red wood added to fire!");
            return;
        }

        // BLUE WOOD
        if (selectedItem.woodType == WoodType.Blue)
        {
            temperatureManager.AddBlueWood();

            ConsumeWood(selectedItem);

            Debug.Log("Blue wood added to fire!");
            return;
        }
    }

    private void ConsumeWood(PickupItem wood)
    {
        // Remove from inventory
        inventory.RemoveSelected();

        // Destroy the physical item
        Destroy(wood.gameObject);
    }
}