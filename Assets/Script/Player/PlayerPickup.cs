using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float interactDistance = 3f;
    public Camera cam;
    public InventorySystem inventory;

    void Update()
    {
        HandleScroll();
        HandleClick();
    }

    void HandleScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        inventory.Scroll(scroll);
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                PickupItem item = hit.collider.GetComponent<PickupItem>();

                if (item != null)
                {
                    inventory.AddItem(item);
                    item.OnPickup();
                    return;
                }
            }

            // If not hitting item → try placing
            PlaceItem();
        }
    }

    void PlaceItem()
    {
        PickupItem selected = inventory.GetSelectedItem();
        if (selected == null) return;

        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
        RaycastHit hit;

        Vector3 dropPosition;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // small safe offset above surface
            dropPosition = hit.point + hit.normal * 0.05f;
        }
        else
        {
            dropPosition = cam.transform.position + cam.transform.forward * interactDistance;
        }

        selected.OnDrop(dropPosition);
        inventory.RemoveSelected();
    }



}
