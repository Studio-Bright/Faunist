using UnityEngine;
using System.Collections.Generic;

public class PotionTemperatureManager : MonoBehaviour
{
    private int totalTemperature = 0;
    private int completedCount = 0;

    public PotionState finalTemperature;
    public bool temperatureReady = false;

    private List<EightSidedPuzzle> puzzles = new List<EightSidedPuzzle>();

    public void RegisterPuzzle(EightSidedPuzzle puzzle)
    {
        if (!puzzles.Contains(puzzle))
            puzzles.Add(puzzle);
    }

    public void RegisterResult(int value)
    {
        totalTemperature += value;
        completedCount++;

        if (completedCount == 3)
            EvaluateFinalTemperature();
    }

    void EvaluateFinalTemperature()
    {
        finalTemperature = ConvertValueToState(totalTemperature);
        temperatureReady = true;

        Debug.Log("Final temperature: " + finalTemperature);

        ResetAllPuzzles();
    }

    void ResetAllPuzzles()
    {
        foreach (var puzzle in puzzles)
            puzzle.ResetPuzzle();

        totalTemperature = 0;
        completedCount = 0;
    }

    PotionState ConvertValueToState(int value)
    {
        if (value <= -2) return PotionState.Cold;
        if (value == -1) return PotionState.Warm;
        if (value <= 1) return PotionState.Hot;
        return PotionState.Boiling;
    }
}