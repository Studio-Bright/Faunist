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

    public void TryPlaceItem(CraftItem item, int slotIndex, Vector3 position)
    {
        if (currentItems[slotIndex] != null) return;
        if (item.isPlacedOnTable) return;

        item.isPlacedOnTable = true;
        item.currentTable = this; // ✅ IMPORTANT

        PickupItem pickup = item.GetComponent<PickupItem>();

        if (pickup != null)
        {
            pickup.rb.linearVelocity = Vector3.zero;
            pickup.rb.angularVelocity = Vector3.zero;
            pickup.rb.isKinematic = true;
            pickup.rb.useGravity = false;
        }

        item.transform.position = position;

        currentItems[slotIndex] = item;

        Debug.Log("Item placed in slot " + slotIndex);
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

    public void RemoveItem(CraftItem item)
    {
        for (int i = 0; i < currentItems.Length; i++)
        {
            if (currentItems[i] == item)
            {
                currentItems[i] = null;
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
