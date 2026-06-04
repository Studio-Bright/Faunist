using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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
        PickupItem bottle = other.GetComponentInParent<PickupItem>();

        if (bottle == null)
        {
            return;
        }
        cauldron.SnapBottle(
            bottle,
            transform.position
        );
        slotCollider.enabled = false;
    }

    public void EnableSlot()
    {
        slotCollider.enabled = true;
    }
}
