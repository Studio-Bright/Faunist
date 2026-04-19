using UnityEngine;

public class CraftItem : MonoBehaviour
{
    public string itemName;
    public SynthesisTable currentTable;

    [HideInInspector] public bool isPlacedOnTable = false;

    // Snap this item to a position (used by table)
    public void SnapToPosition(Vector3 pos)
    {
        transform.position = pos;
        isPlacedOnTable = true;
    }
}
