using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public List<PickupItem> items = new List<PickupItem>();
    public int selectedIndex = 0;

    [Header("UI")]
    public GameObject slotPrefab;
    public Transform slotParent;

    private List<GameObject> slotUI = new List<GameObject>();

    // Add item and create slot
    public void AddItem(PickupItem item)
    {
        items.Add(item);
        selectedIndex = items.Count - 1; // new item selected

        // create UI slot
        GameObject slot = Instantiate(slotPrefab, slotParent);
        slotUI.Add(slot);

        // Set icon in child Image
        Image iconImage = slot.transform.Find("Icon")?.GetComponent<Image>();
        if (iconImage != null && item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true; // make sure it's visible
        }

        UpdateUI();
    }


    // Remove currently selected item
    public void RemoveSelected()
    {
        if (items.Count == 0) return;

        items.RemoveAt(selectedIndex);

        Destroy(slotUI[selectedIndex]);
        slotUI.RemoveAt(selectedIndex);

        // Clamp selection: if last item removed, move selection back
        if (selectedIndex >= items.Count)
            selectedIndex = items.Count - 1;

        // If inventory empty, reset to -1
        if (selectedIndex < 0)
            selectedIndex = 0;

        UpdateUI();
    }


    public PickupItem GetSelectedItem()
    {
        if (items.Count == 0) return null;
        return items[selectedIndex];
    }

    public void Scroll(float scrollInput)
    {
        if (items.Count == 0) return;

        if (scrollInput > 0) 
            selectedIndex = (selectedIndex - 1 + items.Count) % items.Count;
        else if (scrollInput < 0) 
            selectedIndex = (selectedIndex + 1) % items.Count;

        UpdateUI();
    }

    private void UpdateUI()
    {
           for (int i = 0; i < slotUI.Count; i++)
            {
                // Set overlay
                Transform overlay = slotUI[i].transform.Find("Overlay");
                if (overlay != null)
                    overlay.gameObject.SetActive(i == selectedIndex);
            }
        
    }

}
