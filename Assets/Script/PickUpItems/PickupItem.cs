using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupItem : MonoBehaviour
{
    public string itemName;

    public Rigidbody rb;
    public Sprite icon;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnPickup()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        gameObject.SetActive(false);
    }

    public void OnDrop(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Try snapping to synthesis table
        SynthesisTable[] tables = FindObjectsByType<SynthesisTable>(FindObjectsSortMode.None);

        foreach (var table in tables)
        {
            if (table.TrySnapItem(this))
            {
                return; // stop if snapped
            }
        }
    }

}