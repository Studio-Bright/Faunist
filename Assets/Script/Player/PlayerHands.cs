using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform handPoint;

    private PickupItem currentItem;


    // =====================================================
    // SHOW ITEM
    // =====================================================

    public void ShowItem(PickupItem item)
    {
        if (currentItem != null && currentItem != item)
        {
            currentItem.SetVisible(false);
            currentItem.transform.SetParent(null);
        }


        currentItem = item;


        if (item == null)
            return;


        if (handPoint == null)
        {
            Debug.LogWarning("PlayerHands: Hand Point is not assigned.");

            return;
        }


        item.OnPickup(handPoint);
    }


    // =====================================================
    // CLEAR
    // =====================================================

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


    // =====================================================
    // CURRENT ITEM
    // =====================================================

    public PickupItem CurrentItem => currentItem;
}