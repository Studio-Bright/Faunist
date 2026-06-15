using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public InventorySystem inventory;

    [Header("Throw Settings")]
    public KeyCode throwKey = KeyCode.T;
    public float throwForce = 15f;
    public float upwardForce = 1f;

    private void Update()
    {
        if (Input.GetKeyDown(throwKey))
        {
            ThrowSelectedItem();
        }
    }

    private void ThrowSelectedItem()
    {
        PickupItem item = inventory.GetSelectedItem();

        if (item == null)
            return;

        // Remove from inventory first
        inventory.RemoveSelected();

        // Position item slightly in front of camera
        Vector3 spawnPos =
            cam.transform.position +
            cam.transform.forward * 0.8f;

        item.OnDrop(spawnPos);

        Rigidbody rb = item.rb;

        if (rb != null)
        {
            Vector3 throwDirection =
                cam.transform.forward +
                cam.transform.up * upwardForce;

            rb.AddForce(
                throwDirection.normalized * throwForce,
                ForceMode.Impulse);
        }
    }
}