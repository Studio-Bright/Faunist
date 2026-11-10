using UnityEngine;

public class WaterTap : MonoBehaviour, IUniversalInteractable
{
    [Header("References")]
    [SerializeField] private GameObject vfxTap;
    [SerializeField] private WaterSource sinkSource;

    [SerializeField] private bool isPouring;

    public bool ShowInteractionPrompt => true;

    public void Interact(PlayerInteractionContext context)
    {
        Toggle();
    }

    public void AlternateInteract(PlayerInteractionContext context)
    {
    }

    public void Toggle()
    {
        isPouring = !isPouring;

        if (vfxTap != null)
            vfxTap.SetActive(isPouring);

        if (sinkSource != null)
            sinkSource.enabled = isPouring;
    }
}