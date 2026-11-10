using TMPro;
using UnityEngine;

public class ThermometerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text temperatureText;

    public void ShowTemperature(TemperatureManager temperatureManager)
    {
        if (temperatureManager == null)
            return;

        temperatureText.text =
            temperatureManager.GetFinalTemperature().ToString();

        temperatureText.gameObject.SetActive(true);
    }

    public void HideTemperature()
    {
        temperatureText.gameObject.SetActive(false);
    }
}