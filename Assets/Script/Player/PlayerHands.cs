using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    [Header("References")]
    public Transform handPoint;

    private PickupItem currentItem;

    public void ShowItem(PickupItem item)
    {
        // Hide previous item
        if (currentItem != null && currentItem != item)
        {
            currentItem.SetVisible(false);
            currentItem.transform.SetParent(null);
        }

        currentItem = item;

        if (item == null)
            return;

        item.OnPickup(handPoint);
    }

    public void Clear(bool hideItem = true)
    {
        if (currentItem == null)
            return;

        if (hideItem)
        {
            currentItem.SetVisible(false);
        }

        currentItem.transform.SetParent(null);

        currentItem = null;
    }
}