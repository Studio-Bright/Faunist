using UnityEngine;
using static PickupItem;

public class ItemSource : MonoBehaviour
{
    public PickupItem itemPrefab; // what item to give

    public PickupItem GetItem()
    {
        Vector3 spawnPosition = new Vector3(0f, -4f, 0f);
        Quaternion spawnRotation = Quaternion.identity;

        PickupItem newItem = Instantiate(itemPrefab, spawnPosition, spawnRotation);
        return newItem;
    }


}