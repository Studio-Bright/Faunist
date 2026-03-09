using UnityEngine;

public class EmissiveSnail : MonoBehaviour, IInteractable
{
    public string requiredItem;

    public SkinnedMeshRenderer snailRenderer;

    public float emissionStrength = 1f;
    public float emissionIncrease = 2f;

    private Material mat;
    private Color baseEmissionColor;

    void Start()
    {
        mat = snailRenderer.material;

        mat.EnableKeyword("_EMISSION");

        // Store the original emission color
        baseEmissionColor = mat.GetColor("_EmissionColor");

        UpdateEmission();
    }

    public void Interact(PlayerInteraction player)
    {
        PickupItem selectedItem = player.inventory.GetSelectedItem();

        if (selectedItem == null)
        {
            Debug.Log("No item selected.");
            return;
        }

        if (selectedItem.itemName != requiredItem)
        {
            Debug.Log("Wrong item.");
            return;
        }

        IncreaseEmission();
        player.inventory.RemoveSelected();
    }

    void IncreaseEmission()
    {
        emissionStrength += emissionIncrease;
        UpdateEmission();

        Debug.Log("Snail glowing stronger!");
    }

    void UpdateEmission()
    {
        mat.SetColor("_EmissionColor", baseEmissionColor * emissionStrength);
    }
}