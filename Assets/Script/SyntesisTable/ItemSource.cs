using UnityEngine;
using static PickupItem;

public class ItemSource : MonoBehaviour
{
    public PickupItem itemPrefab; // what item to give

    public PickupItem GetItem()
    {
        // Create a fresh instance (not in world)
        PickupItem newItem = Instantiate(itemPrefab);
        return newItem;
    }

   
}