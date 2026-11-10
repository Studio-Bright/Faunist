using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private List<PickupItem> items = new List<PickupItem>();

    [SerializeField] private int selectedIndex = -1;

    [Header("UI")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;

    [Header("Hands")]
    [SerializeField] private PlayerHands playerHands;

    private readonly List<Image> icons = new List<Image>();

    private readonly List<GameObject> overlays = new List<GameObject>();


    // =====================================================
    // PUBLIC API
    // =====================================================

    public PickupItem GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count)
        {
            return null;
        }

        return items[selectedIndex];
    }


    // =====================================================
    // ADD ITEM
    // =====================================================
    public bool IsFull()
    {
        return items.Count >= 5;
    }
    public void AddItem(PickupItem item)
    {
        // Maximum 5 items
        if (items.Count >= 5)
        {
            Debug.Log("Inventory is full!");
            return;
        }

        if (item == null)
            return;

        items.Add(item);
        selectedIndex = items.Count - 1;

        GameObject slot = Instantiate(slotPrefab, slotParent);

        Image icon = slot.transform.Find("Icon")?.GetComponent<Image>();
        GameObject overlay = slot.transform.Find("Overlay")?.gameObject;

        icons.Add(icon);
        overlays.Add(overlay);

        if (icon != null && item.icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }

        UpdateUI();
        RefreshHands();
    }


    // =====================================================
    // REMOVE SELECTED
    // =====================================================

    public void RemoveSelected()
    {
        if (items.Count == 0 || selectedIndex < 0 || selectedIndex >= items.Count)
        {
            return;
        }


        int removedIndex = selectedIndex;


        // Clear the currently displayed hand item.
        if (playerHands != null)
        {
            playerHands.Clear();
        }


        items.RemoveAt(removedIndex);


        // Remove inventory UI slot.
        if (slotParent != null && removedIndex < slotParent.childCount)
        {
            Destroy(slotParent.GetChild(removedIndex).gameObject);
        }


        if (removedIndex < icons.Count)
        {
            icons.RemoveAt(removedIndex);
        }

        if (removedIndex < overlays.Count)
        {
            overlays.RemoveAt(removedIndex);
        }


        // No items left.
        if (items.Count == 0)
        {
            selectedIndex = -1;

            if (playerHands != null)
            {
                playerHands.Clear(false);
            }

            UpdateUI();

            return;
        }


        // Keep selection valid.
        if (removedIndex >= items.Count)
        {
            selectedIndex = items.Count - 1;
        }
        else
        {
            selectedIndex = removedIndex;
        }


        UpdateUI();
        RefreshHands();
    }


    // =====================================================
    // SCROLL
    // =====================================================

    public void Scroll(int direction)
    {
        if (items.Count == 0)
            return;


        selectedIndex = (selectedIndex + direction + items.Count) % items.Count;


        UpdateUI();
        RefreshHands();
    }


    // =====================================================
    // SELECT ITEM
    // =====================================================

    public void SetSelectedItem(PickupItem item)
    {
        if (item == null)
            return;


        int index = items.IndexOf(item);


        if (index == -1)
        {
            Debug.LogWarning("Item not found in inventory!");

            return;
        }


        selectedIndex = index;

        UpdateUI();
        RefreshHands();
    }


    // =====================================================
    // UI
    // =====================================================

    private void UpdateUI()
    {
        for (int i = 0; i < overlays.Count; i++)
        {
            if (overlays[i] != null)
            {
                overlays[i].SetActive(i == selectedIndex);
            }
        }
    }


    // =====================================================
    // HANDS
    // =====================================================

    private void RefreshHands()
    {
        if (playerHands == null)
            return;


        PickupItem item = GetSelectedItem();


        if (item == null)
        {
            playerHands.Clear();
            return;
        }


        if (!item.holdableInHands)
        {
            item.SetVisible(false);

            playerHands.Clear();

            return;
        }


        playerHands.ShowItem(item);
    }
}