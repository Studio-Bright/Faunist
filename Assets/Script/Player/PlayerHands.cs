using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    [Header("References")]
    public Transform handPoint;

    private PickupItem currentItem;

    public void ShowItem(PickupItem item)
    {
        Clear();

        if (item == null)
            return;

        currentItem = item;

        item.OnPickup(handPoint);
    }

    public void Clear()
    {
        if (currentItem == null)
            return;

        currentItem.transform.SetParent(null);

        currentItem = null;
    }
}