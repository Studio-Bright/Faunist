using UnityEngine;

public class PhysicalStatePuzzle : MonoBehaviour
{
    public CauldronInventory cauldron;

    public RotatableItem cube1;
    public RotatableItem cube2;
    public RotatableItem cube3;
    public RotatableItem cube4;

    public void CheckPuzzle()
    {
        if (cube1 == null ||
            cube2 == null ||
            cube3 == null ||
            cube4 == null)
        {
            Debug.LogWarning("PhysicalStatePuzzle is missing cubes.");
            return;
        }

        if (cube1.IsRotating() ||
            cube2.IsRotating() ||
            cube3.IsRotating() ||
            cube4.IsRotating())
        {
            Debug.Log("Cannot check puzzle while cubes are rotating.");
            return;
        }

        int r1 = cube1.GetRotationIndex();
        int r2 = cube2.GetRotationIndex();
        int r3 = cube3.GetRotationIndex();
        int r4 = cube4.GetRotationIndex();

        Debug.Log(
            $"Checking puzzle: {r1}, {r2}, {r3}, {r4}"
        );

        if (IsValidPattern(r1, r2, r3, r4))
        {
            Debug.Log("Puzzle valid -> Brew");

            PhysicalState state = Convert(r4);

            if (cauldron != null)
            {
                cauldron.Brew(state);
            }
        }
        else
        {
            Debug.Log("Puzzle invalid.");
        }
    }

    private bool IsValidPattern(
    int r1,
    int r2,
    int r3,
    int r4)
    {
        int[] states = { r1, r2, r3, r4 };

        bool duplicateUsed = false;

        for (int i = 1; i < states.Length; i++)
        {
            int previous = states[i - 1];
            int current = states[i];

            // Same state
            if (current == previous)
            {
                // Only ONE duplicated pair is allowed
                if (duplicateUsed)
                    return false;

                duplicateUsed = true;

                // Three identical states in a row
                if (i >= 2 && states[i - 2] == current)
                    return false;

                continue;
            }

            // Different state must move forward
            // Solid -> Liquid -> Gas -> Essence -> Solid
            int expected = (previous + 1) % 4;

            if (current != expected)
                return false;
        }

        return true;
    }
    private PhysicalState Convert(int index)
    {
        switch (index)
        {
            case 0:
                return PhysicalState.Solid;

            case 1:
                return PhysicalState.Liquid;

            case 2:
                return PhysicalState.Gas;

            case 3:
                return PhysicalState.Essence;

            default:
                return PhysicalState.Solid;
        }
    }
}

public enum PhysicalState
{
    Solid,
    Liquid,
    Gas,
    Essence
}