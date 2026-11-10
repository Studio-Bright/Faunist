using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class WaterSource : MonoBehaviour, IUniversalInteractable
{
    public CauldronWater waterBucket;

    public bool ShowInteractionPrompt => true;

    public void Interact(PlayerInteractionContext context)
    {
        PickupItem item = context.SelectedItem;

        if (item == null)
        {
            Debug.Log("No item selected!");
            return;
        }

        if (item.itemType != PickupItem.ItemType.Bucket)
        {
            Debug.Log("You need a bucket!");
            return;
        }

        item.containedLiquid = PickupItem.LiquidType.Water;
        waterBucket.FillWater();

        Debug.Log("Bucket filled with water!");
    }

    public void AlternateInteract(PlayerInteractionContext context) { }
}
