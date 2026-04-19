using UnityEngine;
public class PickupItem : MonoBehaviour
{
    public string itemName;

    public Rigidbody rb;
    public Sprite icon;

    public ItemType itemType;

    public LiquidType containedLiquid = LiquidType.None;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


public void OnPickup()
    {
        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

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

     
    }

}