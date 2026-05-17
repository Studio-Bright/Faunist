using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName;

    public Sprite icon;

    public ItemType itemType;

    public LiquidType containedLiquid =
        LiquidType.None;

    [Header("Physics")]
    public Rigidbody rb;

    [Header("Hold Settings")]
    public bool holdableInHands = false;

    [Header("Hand Transform")]
    public Vector3 handPositionOffset;
    public Vector3 handRotationOffset;
    public Vector3 handScale = Vector3.one;

    public enum ItemType
    {
        Ingredient,
        Bucket
    }

    public enum LiquidType
    {
        None,
        Water
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Pickup into hand
    public void OnPickup(Transform handParent)
    {
        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(handParent);

        transform.localPosition = handPositionOffset;

        transform.localRotation =
            Quaternion.Euler(handRotationOffset);

        transform.localScale = handScale;

        SetColliderState(false);
    }

    // Pickup into inventory
    public void OnPickup()
    {
        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        SetColliderState(false);
    }

    public void OnDrop(Vector3 position)
    {
        transform.SetParent(null);

        transform.position = position;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        SetColliderState(true);
    }

    private void SetColliderState(bool state)
    {
        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = state;
    }

    public virtual void Use(PlayerInteraction player)
    {
        Debug.Log(itemName + " has no use function.");
    }
}