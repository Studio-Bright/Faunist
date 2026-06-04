using UnityEngine;

public class Thermometr : MonoBehaviour, IInteractable
{
    [SerializeField] private PotionTemperatureManager temperatureManager;

    public void Interact(PlayerInteraction player)
    {
        if (temperatureManager != null)
        {
            temperatureManager.ResetAllPuzzles();
            Debug.Log("Temperature puzzle reset.");
        }
    }
}