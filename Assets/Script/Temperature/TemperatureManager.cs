using UnityEngine;
using UnityEngine.VFX;

public class TemperatureManager : MonoBehaviour
{
    [Header("Temperature")]
    [SerializeField] private int temperatureLevel = 0;

    [SerializeField] public PotionState finalTemperature;

    public bool temperatureReady = false;

    [Header("Thermometer")]
    [SerializeField] private Transform thermometerMercury;
    [SerializeField] private float fillSpeed = 2f;

    private float targetScaleZ = 0f;

    [Header("Fire VFX")]
    [SerializeField] private VisualEffect fireVFX;

    [SerializeField] private string flameSizeParameter = "Flame Size";
    [SerializeField] private string flameRateParameter = "FlameRate";

    [SerializeField] public float flameChangeSpeed = 2f;
    [SerializeField] public float flameRateChangeSpeed = 12f;
    // Flame Size
    private float targetFlameSize = 0f;
    private float currentFlameSize = 0f;

    // Flame Rate
    private float targetFlameRate = 0f;
    private float currentFlameRate = 0f;


    private void Start()
    {
        UpdateTemperature();

        // Set initial VFX values immediately
        currentFlameSize = targetFlameSize;
        currentFlameRate = targetFlameRate;

        if (fireVFX != null)
        {
            fireVFX.SetFloat(
                flameSizeParameter,
                currentFlameSize
            );

            fireVFX.SetFloat(
                flameRateParameter,
                currentFlameRate
            );
        }
    }


    private void Update()
    {
        // -------------------------
        // Thermometer
        // -------------------------

        if (thermometerMercury != null)
        {
            Vector3 scale =
                thermometerMercury.localScale;

            scale.z = Mathf.MoveTowards(
                scale.z,
                targetScaleZ,
                fillSpeed * Time.deltaTime
            );

            thermometerMercury.localScale = scale;
        }


        // -------------------------
        // Fire VFX
        // -------------------------

        if (fireVFX != null)
        {
            // Flame Size
            currentFlameSize = Mathf.MoveTowards(
                currentFlameSize,
                targetFlameSize,
                flameChangeSpeed * Time.deltaTime
            );

            fireVFX.SetFloat(
                flameSizeParameter,
                currentFlameSize
            );


            // Flame Rate
            currentFlameRate = Mathf.MoveTowards(
                currentFlameRate,
                targetFlameRate,
                flameRateChangeSpeed * Time.deltaTime
            );

            fireVFX.SetFloat(
                flameRateParameter,
                currentFlameRate
            );
        }
    }


    public void AddRedWood()
    {
        temperatureLevel++;

        temperatureLevel = Mathf.Clamp(
            temperatureLevel,
            -2,
            2
        );

        UpdateTemperature();

        Debug.Log(
            $"Red wood added | " +
            $"Level: {temperatureLevel} | " +
            $"State: {finalTemperature}"
        );
    }


    public void AddBlueWood()
    {
        temperatureLevel--;

        temperatureLevel = Mathf.Clamp(
            temperatureLevel,
            -2,
            2
        );

        UpdateTemperature();

        Debug.Log(
            $"Blue wood added | " +
            $"Level: {temperatureLevel} | " +
            $"State: {finalTemperature}"
        );
    }


    private void UpdateTemperature()
    {
        switch (temperatureLevel)
        {
            // -------------------------
            // COLD
            // -------------------------
            case -2:

                finalTemperature = PotionState.Cold;

                targetScaleZ = 0.06f;

                targetFlameSize = 0.14f;
                targetFlameRate = 1f;

                break;


            // -------------------------
            // WARM
            // -------------------------
            case -1:

                finalTemperature = PotionState.Warm;

                targetScaleZ = 0.3f;

                targetFlameSize = 1.355f;
                targetFlameRate = 10f;

                break;


            // -------------------------
            // NEUTRAL
            // -------------------------
            case 0:

                finalTemperature = PotionState.Neutral;

                targetScaleZ = 0.54f;

                targetFlameSize = 2.57f;
                targetFlameRate = 25f;

                break;


            // -------------------------
            // HOT
            // -------------------------
            case 1:

                finalTemperature = PotionState.Hot;

                targetScaleZ = 0.77f;

                targetFlameSize = 3.785f;
                targetFlameRate = 40f;

                break;


            // -------------------------
            // BOILING
            // -------------------------
            case 2:

                finalTemperature = PotionState.Boiling;

                targetScaleZ = 1f;

                targetFlameSize = 5f;
                targetFlameRate = 60f;

                break;
        }
    }


    public void EvaluateFinalTemperature()
    {
        temperatureReady = true;

        UpdateTemperature();

        Debug.LogWarning(
            "=== [TEMPERATURE PROCESS COMPLETE] ==="
        );

        Debug.LogWarning(
            $"Temperature Level: {temperatureLevel} | " +
            $"Potion State: {finalTemperature}"
        );
    }


    public PotionState GetFinalTemperature()
    {
        return finalTemperature;
    }


    public int GetTemperatureLevel()
    {
        return temperatureLevel;
    }
}