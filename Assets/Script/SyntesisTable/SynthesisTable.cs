using UnityEngine;
using System.Collections.Generic;

public class SynthesisTable : MonoBehaviour
{
    [Header("Snap Slots (triangle positions)")]
    public Transform slotA;
    public Transform slotB;
    public Transform slotC;

    [Header("Snap Settings")]
    public float snapDistance = 1f;

    [Header("Recipes")]
    public List<CraftRecipe> recipes;

    [Header("Spawn Point for Crafted Item")]
    public Transform outputSpawn;

    private CraftItem[] currentItems = new CraftItem[3];

    

    

    void Update()
    {
        TryCraft();
    }

    public bool TrySnapItem(PickupItem pickup)
    {
        if (pickup == null) return false;

        CraftItem item = pickup.GetComponent<CraftItem>();
        if (item == null || item.isPlacedOnTable) return false;

        float snapSqr = snapDistance * snapDistance;

        Vector3 pos = pickup.transform.position;

        if (currentItems[0] == null && (pos - slotA.position).sqrMagnitude < snapSqr)
        {
            SnapItem(item, 0, slotA.position);
            return true;
        }

        if (currentItems[1] == null && (pos - slotB.position).sqrMagnitude < snapSqr)
        {
            SnapItem(item, 1, slotB.position);
            return true;
        }

        if (currentItems[2] == null && (pos - slotC.position).sqrMagnitude < snapSqr)
        {
            SnapItem(item, 2, slotC.position);
            return true;
        }

        return false;
    }

    void HandleSnapping()
    {
        PickupItem[] allPickupItems = Object.FindObjectsByType<PickupItem>(FindObjectsSortMode.None);

        foreach (var pickup in allPickupItems)
        {
            CraftItem item = pickup.GetComponent<CraftItem>();
            if (item == null || item.isPlacedOnTable) continue;

            if (Vector3.Distance(pickup.transform.position, slotA.position) < snapDistance && currentItems[0] == null)
            {
                SnapItem(item, 0, slotA.position);
            }
            else if (Vector3.Distance(pickup.transform.position, slotB.position) < snapDistance && currentItems[1] == null)
            {
                SnapItem(item, 1, slotB.position);
            }
            else if (Vector3.Distance(pickup.transform.position, slotC.position) < snapDistance && currentItems[2] == null)
            {
                SnapItem(item, 2, slotC.position);
            }
        }
    }

    void SnapItem(CraftItem item, int slotIndex, Vector3 position)
    {
        if (item.isPlacedOnTable) return;

        item.isPlacedOnTable = true;

        PickupItem pickup = item.GetComponent<PickupItem>();

        if (pickup != null)
        {
            pickup.rb.linearVelocity = Vector3.zero;
            pickup.rb.angularVelocity = Vector3.zero;
            pickup.rb.isKinematic = true;
        }

        item.transform.position = position;

        currentItems[slotIndex] = item;
    }

    void TryCraft()
    {
        // Only if all 3 slots are filled
        if (currentItems[0] == null || currentItems[1] == null || currentItems[2] == null)
            return;

        // Get item names
        List<string> names = new List<string>()
        {
            currentItems[0].itemName,
            currentItems[1].itemName,
            currentItems[2].itemName
        };

        foreach (var recipe in recipes)
        {
            List<string> required = new List<string>() { recipe.input1, recipe.input2, recipe.input3 };

            // Check if all inputs match (order independent)
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
                // Remove old items (disable physics, deactivate, remove from inventory if needed)
                foreach (var item in currentItems)
                {
                    PickupItem pickup = item.GetComponent<PickupItem>();
                    if (pickup != null)
                    {
                        // If you want to return items to inventory instead of destroying, use:
                        // inventory.AddItem(pickup);
                        Destroy(pickup.gameObject); // or gameObject.SetActive(false) to despawn
                    }
                }

                // Reset slots
                currentItems = new CraftItem[3];

                // Spawn new item at spawn point
                Vector3 spawnPos = outputSpawn != null ? outputSpawn.position : transform.position + Vector3.up * 0.5f;
                Instantiate(recipe.outputPrefab, spawnPos, Quaternion.identity);

                Debug.Log("Crafted: " + recipe.outputPrefab.name);
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        

        

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(slotA.position, snapDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(slotB.position, snapDistance);


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(slotC.position, snapDistance);
    }
}
