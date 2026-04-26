using UnityEngine;

public class PhysicalStatePuzzle : MonoBehaviour
{
    public CauldronInventory cauldron;

    public RotatableItem cube1;
    public RotatableItem cube2;
    public RotatableItem cube3;
    public RotatableItem cube4;

    public bool isInteractable = false;
    public void CheckPuzzle()
    {
        Debug.Log("Potion crafted");

        int r1 = cube1.GetRotationIndex();
        int r2 = cube2.GetRotationIndex();
        int r3 = cube3.GetRotationIndex();
        int r4 = cube4.GetRotationIndex();

        if (ValidPair(r3, r4) &&
            ValidPair(r2, r3) &&
            ValidPair(r1, r2))
        {
            cauldron.Brew(Convert(r4));
        }
    }

    bool ValidPair(int previous, int next)
    {
        return previous == next || (previous + 1) % 4 == next;
    }

    PhysicalState Convert(int index)
    {
        switch (index)
        {
            case 0: return PhysicalState.Solid;
            case 1: return PhysicalState.Liquid;
            case 2: return PhysicalState.Gas;
            case 3: return PhysicalState.Essence;
            default: return PhysicalState.Solid;
        }
    }
}