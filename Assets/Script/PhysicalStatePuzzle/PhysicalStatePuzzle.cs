using UnityEngine;

public class PhysicalStatePuzzle : MonoBehaviour
{
    public CauldronInventory cauldron;

    public RotatableItem cube1;
    public RotatableItem cube2;
    public RotatableItem cube3;
    public RotatableItem cube4;

    public bool isInteractable = false;

    private Collider[] cubeColliders;

    private void Awake()
    {
        cubeColliders = new Collider[]
        {
            cube1.GetComponent<Collider>(),
            cube2.GetComponent<Collider>(),
            cube3.GetComponent<Collider>(),
            cube4.GetComponent<Collider>()
        };

        SetInteraction(false);
    }

    public void SetInteraction(bool enabled)
    {
        isInteractable = enabled;

        foreach (Collider col in cubeColliders)
        {
            if (col != null)
                col.enabled = enabled;
        }
    }

    public void CheckPuzzle()
    {
        Debug.Log("CheckPuzzle called");

        int r1 = cube1.GetRotationIndex();
        int r2 = cube2.GetRotationIndex();
        int r3 = cube3.GetRotationIndex();
        int r4 = cube4.GetRotationIndex();
        Debug.Log($"P1={ValidPair(r3, r4)}");
        Debug.Log($"P2={ValidPair(r2, r3)}");
        Debug.Log($"P3={ValidPair(r1, r2)}");
        Debug.Log($"Rotations: {r1} {r2} {r3} {r4}");

        if (ValidPair(r3, r4) &&
            ValidPair(r2, r3) &&
            ValidPair(r1, r2))
        {
            Debug.Log("Puzzle valid -> Brew");
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