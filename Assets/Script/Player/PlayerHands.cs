using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    public Transform handPoint; // assign in inspector (camera child)

    private GameObject currentHeldObject;

    public void ShowItem(PickupItem item)
    {
        Clear();

        if (item == null)
            return;

        GameObject prefabToShow = item.GetHoldVisual();

        if (prefabToShow == null)
        {
            Debug.LogWarning("No hold visual assigned!");
            return;
        }

        currentHeldObject = Instantiate(prefabToShow, handPoint);
        currentHeldObject.transform.localPosition = Vector3.zero;
        currentHeldObject.transform.localRotation = Quaternion.identity;
    }

    public void Clear()
    {
        if (currentHeldObject != null)
        {
            Destroy(currentHeldObject);
        }
    }
}