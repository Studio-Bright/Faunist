using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float interactDistance = 3f;
    public Camera cam;
    public InventorySystem inventory;
    public AnimalEncounterManager animalEncounterManager;

    private void Update()
    {
        HandleClick();
    }

    void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
        RaycastHit hit;

        int interactMask = ~LayerMask.GetMask("CraftLayer");

        if (Physics.Raycast(ray, out hit, interactDistance, interactMask))
        {
            Debug.Log("Hit: " + hit.collider.name);

            Drawer drawer = hit.collider.GetComponentInParent<Drawer>();
            if (drawer != null)
            {
                drawer.Toggle();
                return;
            }

            SwingDoor door = hit.collider.GetComponentInParent<SwingDoor>();
            if (door != null)
            {
                door.Toggle();
                return;
            }

            if (hit.collider.TryGetComponent(out Animal animal))
            {
                TryUsePotionOnAnimal(animal);
                return;
            }

            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(GetComponent<PlayerInteraction>());
                return;
            }

            PickupItem pickupItem = hit.collider.GetComponentInParent<PickupItem>();
            if (pickupItem != null)
            {
                CraftItem craftItem = pickupItem.GetComponent<CraftItem>();

                if (craftItem != null && craftItem.isPlacedOnTable)
                {
                    craftItem.currentTable?.RemoveItem(craftItem);
                }

                inventory.AddItem(pickupItem);
                pickupItem.OnPickup();

                Debug.Log("Picked item");
                return;
            }

            ItemSource source = hit.collider.GetComponent<ItemSource>();
            if (source != null)
            {
                PickupItem newItem = source.GetItem();

                if (newItem != null)
                    inventory.AddItem(newItem);

                return;
            }
        }

        PlaceItem();
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
            dropPosition = hit.point + hit.normal * 0.05f;
        }
        else
        {
            dropPosition = cam.transform.position + cam.transform.forward * interactDistance;
        }

        selected.OnDrop(dropPosition);
        inventory.RemoveSelected();
    }

    void TryUsePotionOnAnimal(Animal animal)
    {
        PickupItem item = inventory.GetSelectedItem();
        if (item == null) return;

        PotionItem potion = item.GetComponent<PotionItem>();

        if (potion == null)
        {
            Debug.Log("This is not a potion!");
            return;
        }

        UsePotionOnAnimal(potion, animal);
    }

    public void UsePotionOnAnimal(PotionItem potion, Animal animal)
    {
        if (potion.targetAnimalID == animal.animalID)
        {
            Debug.Log("✅ Correct potion used!");
            Destroy(potion.gameObject);
            animalEncounterManager.OnAnimalHealed();
        }
        else
        {
            Debug.Log("❌ Wrong potion!");
        }
    }
}