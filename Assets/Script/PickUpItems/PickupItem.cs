using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName;
    public Sprite icon;

    public ItemType itemType;
    public LiquidType containedLiquid = LiquidType.None;

    public WoodType woodType = WoodType.NotWood;

    [Header("Physics")]
    public Rigidbody rb;

    [Header("Hold Settings")]
    public bool holdableInHands = false;

    [Header("Hand Transform")]
    public Vector3 handPositionOffset;
    public Vector3 handRotationOffset;
    public Vector3 handScale = Vector3.one;

    private Renderer[] renderers;
    private Collider[] colliders;

    public enum ItemType
    {
        Ingredient,
        Bucket,
        EmptyBottle,
        Potion
    }

    public enum LiquidType
    {
        None,
        Water
    }
    public enum WoodType
    {
        Red,
        Blue,

        NotWood
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
    }

    public void SetVisible(bool state)
    {
        foreach (Renderer r in renderers)
            r.enabled = state;
    }

    public void SetColliders(bool state)
    {
        foreach (Collider c in colliders)
            c.enabled = state;
    }

    public virtual void OnPickup(Transform handParent)
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

        SetColliders(false);
        SetVisible(true);
    }

    public virtual void OnPickup()
    {
        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(null);

        SetColliders(false);
        SetVisible(false);
    }

    public virtual void OnDrop(Vector3 position)
    {
        transform.SetParent(null);

        transform.position = position;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        SetVisible(true);
        SetColliders(true);
    }

    public virtual void Use(PlayerInteraction player)
    {
        Debug.Log(itemName + " has no use function.");
    }
}