using UnityEngine;

public class EmissiveSnail : MonoBehaviour, IUniversalInteractable
{
    [Header("Required Item")]
    [SerializeField] private string requiredItem;

    [Header("Renderer")]
    [SerializeField] private SkinnedMeshRenderer snailRenderer;

    [Header("Emission")]
    [SerializeField] private float emissionStrength = 1f;
    [SerializeField] private float emissionIncrease = 2f;

    private Material mat;
    private Color baseEmissionColor;

    public bool ShowInteractionPrompt => true;

    private void Start()
    {
        if (snailRenderer == null)
        {
            Debug.LogError($"{name}: EmissiveSnail requires a SkinnedMeshRenderer.");
            return;
        }

        mat = snailRenderer.material;

        mat.EnableKeyword("_EMISSION");

        baseEmissionColor = mat.GetColor("_EmissionColor");

        UpdateEmission();
    }

    public void Interact(PlayerInteractionContext context)
    {
        if (context == null)
            return;

        PickupItem selectedItem = context.SelectedItem;

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

        context.Inventory.RemoveSelected();
    }

    public void AlternateInteract(PlayerInteractionContext context) { }

    private void IncreaseEmission()
    {
        emissionStrength += emissionIncrease;

        UpdateEmission();

        Debug.Log("Snail glowing stronger!");
    }

    private void UpdateEmission()
    {
        if (mat == null)
            return;

        mat.SetColor("_EmissionColor", baseEmissionColor * emissionStrength);
    }
}