using UnityEngine;

public class Thermometr : MonoBehaviour, IUniversalInteractable
{
    [Header("Temperature")]
    [SerializeField] private PotionTemperatureManager temperatureManager;

    public bool ShowInteractionPrompt => true;

    public void Interact(PlayerInteractionContext context)
    {
        if (temperatureManager == null)
            return;

        temperatureManager.ResetAllPuzzles();

        Debug.Log("Temperature puzzle reset.");
    }

    public void AlternateInteract(PlayerInteractionContext context) { }
}