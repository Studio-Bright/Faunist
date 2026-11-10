using UnityEngine;

public class ItemSource : MonoBehaviour, IUniversalInteractable
{
    [Header("Item")]
    public PickupItem itemPrefab;
    public bool ShowInteractionPrompt => true;

    public void Interact(PlayerInteractionContext context)
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning("ItemSource has no itemPrefab assigned.", this);
            return;
        }


        if (context.Inventory == null)
        {
            Debug.LogWarning("ItemSource could not find the player's InventorySystem.");
            return;
        }

        PickupItem newItem = GetItem();

        if (newItem == null)
            return;

        context.Inventory.AddItem(newItem);

        Debug.Log("Received item from source: " + newItem.itemName);
    }

    public void AlternateInteract(PlayerInteractionContext context)
    {
    }

    public PickupItem GetItem()
    {
        Vector3 spawnPosition = new Vector3(0f, -4f, 0f);

        Quaternion spawnRotation = Quaternion.identity;

        PickupItem newItem = Instantiate( itemPrefab, spawnPosition, spawnRotation);

        return newItem;
    }
}
