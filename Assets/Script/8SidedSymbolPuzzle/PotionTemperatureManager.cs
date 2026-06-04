using UnityEngine;
using System.Collections.Generic;

public class PotionTemperatureManager : MonoBehaviour
{
    

    private int totalTemperature = 0;
    private int completedCount = 0;

    public PotionState finalTemperature;
    public bool temperatureReady = false;

    [SerializeField] private Transform thermometerMercury;

    [SerializeField] private float minScaleZ = 0f;
    float targetScaleZ = 0f;
    [SerializeField] private float fillSpeed = 20f;

    private List<EightSidedPuzzle> puzzles = new List<EightSidedPuzzle>();

    void Start()
    {
        targetScaleZ = minScaleZ;
    }
    void Update()
    {
        Vector3 scale = thermometerMercury.localScale;

        scale.z = Mathf.MoveTowards(
            scale.z,
            targetScaleZ,
            fillSpeed * Time.deltaTime);

        thermometerMercury.localScale = scale;
    }
    public void RegisterPuzzle(EightSidedPuzzle puzzle)
    {
        if (!puzzles.Contains(puzzle))
        {
            puzzles.Add(puzzle);
        }
    }

    public void RegisterResult(int value)
    {
        totalTemperature += value;
        completedCount++;

        Debug.Log($"[Temperature Progress] Registered a puzzle result of: {value}. Total Temp is now: {totalTemperature}. Puzzles finished: {completedCount}/3");

      

        if (completedCount == 3)
        {
            EvaluateFinalTemperature();
        }
    }

    void EvaluateFinalTemperature()
    {
        finalTemperature = ConvertValueToState(totalTemperature);
        temperatureReady = true;

        UpdateThermometer();

        Debug.LogWarning($"=== [TEMPERATURE PROCESS COMPLETE] ===");
        Debug.LogWarning($"Final Combined Score: {totalTemperature} | Evaluated Potion State: {finalTemperature}");

        // Only reset after you have successfully parsed and read the values
        //ResetAllPuzzles();
    }

    public void ResetAllPuzzles()
    {
        foreach (var puzzle in puzzles)
        {
            if (puzzle != null)
                puzzle.ResetPuzzle();
        }

        totalTemperature = 0;
        completedCount = 0;

        finalTemperature = PotionState.Cold;
        temperatureReady = false;

        targetScaleZ = minScaleZ;
    }

    PotionState ConvertValueToState(int value)
    {
        // Adjust these numeric ranges based on your balancing goals
        if (value <= 0) return PotionState.Cold;
        if (value == 1) return PotionState.Warm;
        if (value <= 2) return PotionState.Hot;
        return PotionState.Boiling;
    }

    void UpdateThermometer()
    {
        switch (finalTemperature)
        {
            case PotionState.Cold:
                targetScaleZ = 0.2374f;
                break;

            case PotionState.Warm:
                targetScaleZ = 0.4748f;
                break;

            case PotionState.Hot:
                targetScaleZ = 0.7122f;
                break;

            case PotionState.Boiling:
                targetScaleZ = 1f;
                break;
        }
    }
}

