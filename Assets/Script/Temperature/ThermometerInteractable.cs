using UnityEngine;

public class ThermometerInteractable : MonoBehaviour, IUniversalInteractable
{
    [SerializeField] private TemperatureManager temperatureManager;

    public TemperatureManager TemperatureManager => temperatureManager;

    // We don't want the normal "Press E" prompt for the thermometer.
    public bool ShowInteractionPrompt => false;

    public void Interact(PlayerInteractionContext context)
    {
        // Thermometer doesn't need an interaction.
    }

    public void AlternateInteract(PlayerInteractionContext context)
    {
        // Thermometer doesn't need an alternate interaction.
    }
}