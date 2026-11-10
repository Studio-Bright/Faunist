using UnityEngine;

public class BottleSlot : MonoBehaviour
{
    public CauldronInventory cauldron;

    private Collider slotCollider;

    private void Awake()
    {
        slotCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cauldron == null)
            return;

        PickupItem item =
            other.GetComponentInParent<PickupItem>();

        if (item == null)
            return;

        // Only bottles and already-created potions
        // are allowed in this slot.
        if (item.itemType != PickupItem.ItemType.EmptyBottle &&
            item.itemType != PickupItem.ItemType.Potion)
        {
            return;
        }

        // Don't accept another item while the slot
        // is already occupied.
        if (cauldron.currentBottle != null)
            return;

        cauldron.SnapBottle(
            item,
            transform.position
        );

        if (slotCollider != null)
            slotCollider.enabled = false;
    }

    public void EnableSlot()
    {
        if (slotCollider != null)
            slotCollider.enabled = true;
    }

    public void DisableSlot()
    {
        if (slotCollider != null)
            slotCollider.enabled = false;
    }
}