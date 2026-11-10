
using UnityEngine;
using static PickupItem;

public class WoodPlace : MonoBehaviour, IUniversalInteractable
{
    [SerializeField] private TemperatureManager temperatureManager;

    public bool ShowInteractionPrompt => true;

    public void Interact(PlayerInteractionContext context)
    {
        if (temperatureManager == null)
        {
            Debug.LogError("WoodPlace: TemperatureManager is not assigned!");
            return;
        }

        PickupItem selectedItem = context.SelectedItem;

        if (selectedItem == null)
        {
            Debug.Log("You need to select some wood first.");
            return;
        }

        if (selectedItem.woodType == WoodType.NotWood)
        {
            Debug.Log("This item is not wood.");
            return;
        }

        if (selectedItem.woodType == WoodType.Red)
        {
            temperatureManager.AddRedWood();
            ConsumeWood(context, selectedItem);
            Debug.Log("Red wood added to fire!");
            return;
        }

        if (selectedItem.woodType == WoodType.Blue)
        {
            temperatureManager.AddBlueWood();
            ConsumeWood(context, selectedItem);
            Debug.Log("Blue wood added to fire!");
        }
    }

    public void AlternateInteract(PlayerInteractionContext context) { }

    private void ConsumeWood(PlayerInteractionContext context, PickupItem wood)
    {
        context.Inventory.RemoveSelected();
        Destroy(wood.gameObject);
    }
}

