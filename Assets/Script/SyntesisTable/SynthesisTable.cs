using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesisTable : MonoBehaviour
{
    [Header("Snap Slots (triangle positions)")]
    public Transform slotA;
    public Transform slotB;
    public Transform slotC;
    public Collider colliderA;
    public Collider colliderB;
    public Collider colliderC;
    private Collider[] slotColliders;

    [Header("Segments")]
    public PathSegment segmentAB;
    public PathSegment segmentBC;
    public PathSegment segmentCA;

    [Header("Center Segment")]
    public PathSegment centerSegment;

    public GameObject syntesisVFXCraft;
    public Transform syntesisVFXSpawnPoint;

    [Header("Recipes")]
    public List<CraftRecipe> recipes;

    [Header("Spawn Point for Crafted Item")]
    public Transform outputSpawn;

    private CraftItem[] currentItems = new CraftItem[3];

    [Header("Glow Connections")]
    [SerializeField] private List<SynthesisConnection> connections = new List<SynthesisConnection>();

    private bool isCrafting = false;
    void Awake()
    {
        slotColliders = new Collider[3] { colliderA, colliderB, colliderC };
    }


    void Update()
    {
        TryCraft();
    }

    public void TryPlaceItem(
    CraftItem item,
    int slotIndex,
    Vector3 position,
    Quaternion rotation
)
    {
        if (currentItems[slotIndex] != null) return;
        if (item.isPlacedOnTable) return;

        item.isPlacedOnTable = true;
        item.currentTable = this;

        PickupItem pickup = item.GetComponent<PickupItem>();

        if (pickup != null)
        {
            pickup.rb.linearVelocity = Vector3.zero;
            pickup.rb.angularVelocity = Vector3.zero;
            pickup.rb.isKinematic = true;
            pickup.rb.useGravity = false;
        }

        item.transform.position = position;

        // STRICT ROTATION
        item.transform.rotation = rotation;

        currentItems[slotIndex] = item;

        if (slotColliders[slotIndex] != null)
            slotColliders[slotIndex].enabled = false;

        RefreshSegments();
        Debug.Log("Item placed in slot " + slotIndex);
    }



    void TryCraft()
    {
        if (isCrafting) return;

        if (currentItems[0] == null ||
            currentItems[1] == null ||
            currentItems[2] == null)
            return;

        List<string> names = new List<string>()
    {
        currentItems[0].itemName,
        currentItems[1].itemName,
        currentItems[2].itemName
    };

        foreach (var recipe in recipes)
        {
            List<string> required = new List<string>()
        {
            recipe.input1,
            recipe.input2,
            recipe.input3
        };

            bool canCraft = true;

            foreach (var r in required)
            {
                if (!names.Contains(r))
                {
                    canCraft = false;
                    break;
                }
            }

            if (canCraft)
            {
                StartCoroutine(CraftRoutine(recipe));
                break;
            }
        }
    }


    IEnumerator CraftRoutine(CraftRecipe recipe)
    {
        isCrafting = true;

        yield return new WaitForSeconds(1f);

        if (centerSegment != null)
            centerSegment.SetActive(true);

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < currentItems.Length; i++)
        {
            var item = currentItems[i];

            if (item == null) continue;

            PickupItem pickup = item.GetComponent<PickupItem>();
            if (pickup != null)
                Destroy(pickup.gameObject);
        }

        currentItems = new CraftItem[3];

        RefreshSegments(); 

        Vector3 spawnPos = outputSpawn != null
            ? outputSpawn.position
            : transform.position + Vector3.up * 0.5f;

        Instantiate(recipe.outputPrefab, spawnPos, Quaternion.identity);
        CraftVFX(syntesisVFXSpawnPoint.position);



        
        ResetSlots();
        isCrafting = false;
    }
    public void RemoveItem(CraftItem item)
    {
        for (int i = 0; i < currentItems.Length; i++)
        {
            if (currentItems[i] == item)
            {
                currentItems[i] = null;

                // 🟢 Re-enable collider
                if (slotColliders[i] != null)
                    slotColliders[i].enabled = true;

                break;
            }
        }

        item.isPlacedOnTable = false;
        item.currentTable = null;

        PickupItem pickup = item.GetComponent<PickupItem>();
        if (pickup != null)
        {
            pickup.rb.isKinematic = false;
            pickup.rb.useGravity = true;
            pickup.rb.linearVelocity = Vector3.zero;
            pickup.rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Item removed from table");
        ResetSlots();
        RefreshSegments();
    }

    void RefreshSegments()
    {
        bool a = currentItems[0] != null;
        bool b = currentItems[1] != null;
        bool c = currentItems[2] != null;

        // AB (0-1)
        if (segmentAB != null)
            segmentAB.SetActive(a && b);

        // BC (1-2)
        if (segmentBC != null)
            segmentBC.SetActive(b && c);

        // CA (2-0)
        if (segmentCA != null)
            segmentCA.SetActive(c && a);

        if (centerSegment != null)
            centerSegment.SetActive(false);
    }

    void ResetSlots()
    {
        for (int i = 0; i < slotColliders.Length; i++)
        {
            if (slotColliders[i] != null)
                slotColliders[i].enabled = true;
        }
    }

    public void CraftVFX(Vector3 position)
    {
        GameObject vfx = Instantiate(syntesisVFXCraft, position, Quaternion.identity);

        Destroy(vfx, 2f);
    }
}
