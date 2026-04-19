using UnityEngine;

public class WaterSource : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInteraction player)
    {
        PickupItem item = player.inventory.GetSelectedItem();

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

        Debug.Log("Bucket filled with water!");
    }
}