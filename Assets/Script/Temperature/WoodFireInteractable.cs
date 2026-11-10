using UnityEngine;

public class WoodFireInteractable : MonoBehaviour, IUniversalInteractable
{
    [SerializeField] private TemperatureManager temperatureManager;

    public bool ShowInteractionPrompt => true;

    public void Interact(PlayerInteractionContext context)
    {
        PickupItem item = context.SelectedItem;

        if (item == null)
            return;

        if (item.woodType == PickupItem.WoodType.NotWood)
        {
            Debug.Log("This item is not wood.");
            return;
        }

        switch (item.woodType)
        {
            case PickupItem.WoodType.Red:
                temperatureManager.AddRedWood();
                Consume(context, item);
                break;

            case PickupItem.WoodType.Blue:
                temperatureManager.AddBlueWood();
                Consume(context, item);
                break;
        }
    }

    public void AlternateInteract(PlayerInteractionContext context)
    {
    }

    private void Consume(
        PlayerInteractionContext context,
        PickupItem item)
    {
        context.Inventory.RemoveSelected();

        Destroy(item.gameObject);
    }
}