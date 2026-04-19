using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public List<PickupItem> items = new List<PickupItem>();
    public int selectedIndex = -1;

    [Header("UI")]
    public GameObject slotPrefab;
    public Transform slotParent;

    private List<Image> icons = new List<Image>();
    private List<GameObject> overlays = new List<GameObject>();

    // Add item and create slot
    public void AddItem(PickupItem item)
    {
        items.Add(item);
        selectedIndex = items.Count - 1;

        GameObject slot = Instantiate(slotPrefab, slotParent);

        // Cache references once (no more Find in Update)
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
    }

    // Remove currently selected item
    public void RemoveSelected()
    {
        if (items.Count == 0 || selectedIndex < 0) return;

        items.RemoveAt(selectedIndex);

        Destroy(slotParent.GetChild(selectedIndex).gameObject);
        icons.RemoveAt(selectedIndex);
        overlays.RemoveAt(selectedIndex);

        // Adjust selection
        if (items.Count == 0)
        {
            selectedIndex = -1;
        }
        else if (selectedIndex >= items.Count)
        {
            selectedIndex = items.Count - 1;
        }

        UpdateUI();
    }

    public PickupItem GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count)
            return null;

        return items[selectedIndex];
    }

    // Use int direction instead of float
    public void Scroll(int direction)
    {
        if (items.Count == 0) return;

        selectedIndex = (selectedIndex + direction + items.Count) % items.Count;
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < overlays.Count; i++)
        {
            if (overlays[i] != null)
                overlays[i].SetActive(i == selectedIndex);
        }
    }

    public void SetSelectedItem(PickupItem item)
    {
        int index = items.IndexOf(item);

        if (index == -1)
        {
            Debug.LogWarning("Item not found in inventory!");
            return;
        }

        selectedIndex = index;
        UpdateUI();
    }
}