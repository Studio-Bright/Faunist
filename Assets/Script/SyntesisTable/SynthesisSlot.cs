using UnityEngine;

public class SynthesisSlot : MonoBehaviour
{
    public int slotIndex;
    public SynthesisTable table;

    private void OnTriggerEnter(Collider other)
    {
        CraftItem item = other.GetComponentInParent<CraftItem>();

        if (item == null) return; 
        if (item.isPlacedOnTable) return;

        table.TryPlaceItem(item, slotIndex, transform.position);
    }
}