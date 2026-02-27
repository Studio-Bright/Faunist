using UnityEngine;
using System.Collections.Generic;

public class PotionTemperatureManager : MonoBehaviour
{
    private int totalTemperature = 0;
    private int completedCount = 0;

    private PotionState requiredState;

    public enum PotionState
    {
        Cold,
        Heated,
        Hot,
        Boiling
    }


    private List<EightSidedPuzzle> puzzles = new List<EightSidedPuzzle>();

    void Start()
    {
        GenerateNewRequirement();
    }

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
        {
            EvaluateFinalTemperature();
        }
    }

    void EvaluateFinalTemperature()
    {
        PotionState finalState = ConvertValueToState(totalTemperature);

        if (finalState == requiredState)
        {
            Debug.Log("That's right!");
            GenerateNewRequirement();
        }
        else
        {
            Debug.Log("Try again!");
        }

        ResetAllPuzzles();
    }

    void ResetAllPuzzles()
    {
        foreach (var puzzle in puzzles)
        {
            puzzle.ResetPuzzle();
        }

        totalTemperature = 0;
        completedCount = 0;
    }

    PotionState ConvertValueToState(int value)
    {
        if (value <= -2) return PotionState.Cold;
        if (value == -1) return PotionState.Heated;
        if (value <= 1) return PotionState.Hot;
        return PotionState.Boiling;
    }

    void GenerateNewRequirement()
    {
        requiredState = (PotionState)Random.Range(0, 4);
        Debug.Log("Required potion state: " + requiredState);
    }
}
