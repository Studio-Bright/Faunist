using UnityEngine;
using System.Collections.Generic;

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


    

    [Header("Recipes")]
    public List<CraftRecipe> recipes;

    [Header("Spawn Point for Crafted Item")]
    public Transform outputSpawn;

    private CraftItem[] currentItems = new CraftItem[3];


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

        Debug.Log("Item placed in slot " + slotIndex);
    }



    void TryCraft()
    {
        if (currentItems[0] == null || currentItems[1] == null || currentItems[2] == null)
            return;

        List<string> names = new List<string>()
        {
            currentItems[0].itemName,
            currentItems[1].itemName,
            currentItems[2].itemName
        };

        foreach (var recipe in recipes)
        {
            List<string> required = new List<string>() { recipe.input1, recipe.input2, recipe.input3 };

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
                for (int i = 0; i < currentItems.Length; i++)
                {
                    var item = currentItems[i];

                    PickupItem pickup = item.GetComponent<PickupItem>();
                    if (pickup != null)
                    {
                        Destroy(pickup.gameObject);
                    }

                    // 🟢 Re-enable slot collider
                    if (slotColliders[i] != null)
                        slotColliders[i].enabled = true;
                }

                currentItems = new CraftItem[3];

                Vector3 spawnPos = outputSpawn != null ? outputSpawn.position : transform.position + Vector3.up * 0.5f;
                Instantiate(recipe.outputPrefab, spawnPos, Quaternion.identity);

                Debug.Log("Crafted: " + recipe.outputPrefab.name);
                break;
            }
        }
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
    }
}
