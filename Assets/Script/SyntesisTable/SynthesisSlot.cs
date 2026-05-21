using UnityEngine;

public class SynthesisSlot : MonoBehaviour
{
    public int slotIndex;
    public SynthesisTable table;

    [Header("Snap Rotation")]
    public Vector3 snapEulerRotation;

    private void OnTriggerEnter(Collider other)
    {
        CraftItem item = other.GetComponentInParent<CraftItem>();

        if (item == null) return;
        if (item.isPlacedOnTable) return;

        Quaternion snapRotation = Quaternion.Euler(snapEulerRotation);

        table.TryPlaceItem(
            item,
            slotIndex,
            transform.position,
            snapRotation
        );
    }
}