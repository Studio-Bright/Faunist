using UnityEngine;

public class PickupItem : MonoBehaviour, IUniversalInteractable
{
    [Header("Item Info")]
    [SerializeField] public string itemName;
    [SerializeField] public Sprite icon;
    [SerializeField] public ItemType itemType;

    [SerializeField] public LiquidType containedLiquid = LiquidType.None;

    [SerializeField] public WoodType woodType = WoodType.NotWood;
    [Header("Physics")]
    [SerializeField] public Rigidbody rb;

    [Header("Collision Resolution")]
    [SerializeField] private LayerMask placementCollisionMask = ~0;
    public bool IsHeld { get; private set; }


    public bool ShowInteractionPrompt => true;


    public virtual void Interact(PlayerInteractionContext context)
    {
        if (IsHeld)
            return;

        if (context == null || context.Inventory == null)
            return;

        // Inventory full - do nothing
        if (context.Inventory.IsFull())
        {
            Debug.Log("Inventory is full!");
            return;
        }

        // Synthesis table
        CraftItem craftItem = GetComponent<CraftItem>();

        if (craftItem != null && craftItem.isPlacedOnTable)
        {
            craftItem.currentTable?.RemoveItem(craftItem);
        }

        // Cauldron bottle/potion slot
        if (CurrentCauldron != null)
        {
            CurrentCauldron.RemoveItem(this);
        }

        OnPickup();

        context.Inventory.AddItem(this);

        Debug.Log("Picked item: " + itemName);
    }

    public virtual void AlternateInteract(PlayerInteractionContext context) { }

    // =====================================================
    // HOLD SETTINGS
    // =====================================================

    [Header("Hold Settings")]
    [SerializeField] public bool holdableInHands = false;

    [Header("Hand Transform")]
    [SerializeField] public Vector3 handPositionOffset;

    [SerializeField] public Vector3 handRotationOffset;

    [SerializeField] public Vector3 handScale = Vector3.one;


    private Renderer[] renderers;
    private Collider[] colliders;


    // =====================================================
    // ENUMS
    // =====================================================

    public enum ItemType
    {
        Ingredient,
        Bucket,
        EmptyBottle,
        Potion,
        Wood
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


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        renderers = GetComponentsInChildren<Renderer>(true);

        colliders = GetComponentsInChildren<Collider>(true);
    }


    // =====================================================
    // VISIBILITY
    // =====================================================

    public void SetVisible(bool state)
    {
        if (renderers == null)
            return;

        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.enabled = state;
        }
    }


    // =====================================================
    // COLLIDERS
    // =====================================================

    public void SetColliders(bool state)
    {
        if (colliders == null)
            return;

        foreach (Collider c in colliders)
        {
            if (c != null)
                c.enabled = state;
        }
    }


    // =====================================================
    // PICKUP WITH HAND
    // =====================================================

    public virtual void OnPickup(Transform handParent)
    {
        IsHeld = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        transform.SetParent(handParent);

        transform.localPosition = handPositionOffset;

        transform.localRotation = Quaternion.Euler(handRotationOffset);

        transform.localScale = handScale;


        SetColliders(false);
        SetVisible(true);
    }


    // =====================================================
    // PICKUP
    // =====================================================

    public virtual void OnPickup()
    {
        IsHeld = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        transform.SetParent(null);

        SetColliders(false);
        SetVisible(false);
    }


    // =====================================================
    // DROP
    // =====================================================

    // =====================================================
    // DROP (FIXED)
    // =====================================================

    public virtual void OnDrop(Vector3 position)
    {
        IsHeld = false;

        transform.SetParent(null);

        SetVisible(true);
        SetColliders(true);

        transform.position = position;

        Physics.SyncTransforms();

        ResolvePenetration();

        Physics.SyncTransforms();

        if (rb != null)
        {
            // 1. Convert back to dynamic non-kinematic FIRST
            rb.isKinematic = false;
            rb.useGravity = true;

            // 2. Clear velocities AFTER turning kinematic off
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // 3. Set collision detection mode once object is dynamic
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }


    // =====================================================
    // COLLISION RESOLUTION
    // =====================================================

    // =====================================================
    // COLLISION RESOLUTION (FIXED FOR MESH COLLIDERS)
    // =====================================================

    private void ResolvePenetration()
    {
        Collider[] myColliders = GetComponentsInChildren<Collider>(true);
        if (myColliders == null || myColliders.Length == 0) return;

        const int maxIterations = 5;
        const float separationMargin = 0.02f;

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            bool foundPenetration = false;

            foreach (Collider myCollider in myColliders)
            {
                if (myCollider == null || !myCollider.enabled)
                    continue;

                // Use OverlapBox with actual collider orientation and slightly expanded bounds
                Bounds bounds = myCollider.bounds;
                Collider[] nearby = Physics.OverlapBox(
                    bounds.center,
                    bounds.extents + Vector3.one * 0.05f,
                    myCollider.transform.rotation,
                    placementCollisionMask,
                    QueryTriggerInteraction.Ignore
                );

                foreach (Collider other in nearby)
                {
                    if (other == null || other == myCollider) continue;
                    if (other.transform.IsChildOf(transform) || myCollider.transform.IsChildOf(other.transform)) continue;

                    // Attempt standard PhysX penetration resolution (Works for Convex, Primitive, and Terrain colliders)
                    if (Physics.ComputePenetration(
                        myCollider, myCollider.transform.position, myCollider.transform.rotation,
                        other, other.transform.position, other.transform.rotation,
                        out Vector3 direction, out float distance))
                    {
                        transform.position += direction * (distance + separationMargin);
                        Physics.SyncTransforms();
                        foundPenetration = true;
                    }
                    // FALLBACK FOR NON-CONVEX MESH COLLIDERS (Physics.ComputePenetration fails for non-convex MeshColliders)
                    else if (other is MeshCollider meshCol && !meshCol.convex)
                    {
                        // Raycast upward/downward from bounds center to find the surface of the non-convex mesh
                        Ray ray = new Ray(bounds.center + Vector3.up * 0.5f, Vector3.down);
                        if (meshCol.Raycast(ray, out RaycastHit hit, 1.0f))
                        {
                            float bottomY = bounds.min.y;
                            if (bottomY < hit.point.y)
                            {
                                // Push the item up out of the non-convex mesh surface
                                float penetrationDepth = hit.point.y - bottomY;
                                transform.position += Vector3.up * (penetrationDepth + separationMargin);
                                Physics.SyncTransforms();
                                foundPenetration = true;
                            }
                        }
                    }
                }
            }

            if (!foundPenetration)
                break;
        }
    }
    // =====================================================
    // USE
    // =====================================================

    public virtual void Use(PlayerInteractionContext context)
    {
        Debug.Log(itemName + " has no use function.");
    }

    public CauldronInventory CurrentCauldron { get; private set; }

    public void SetCauldron(CauldronInventory cauldron)
    {
        CurrentCauldron = cauldron;
    }
}