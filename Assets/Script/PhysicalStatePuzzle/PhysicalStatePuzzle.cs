using UnityEngine;

public class PhysicalStatePuzzle : MonoBehaviour
{
    public CauldronInventory cauldron;

    public RotatableItem cube1;
    public RotatableItem cube2;
    public RotatableItem cube3;
    public RotatableItem cube4;

    public GameObject[] resultPrefabs; 
    public Transform spawnPoint;

    public bool isInteractable = false;

    public void CheckPuzzle()
    {
        int r1 = cube1.GetRotationIndex();
        int r2 = cube2.GetRotationIndex();
        int r3 = cube3.GetRotationIndex();
        int r4 = cube4.GetRotationIndex();

        if (ValidPair(r3, r4) &&
            ValidPair(r2, r3) &&
            ValidPair(r1, r2))
        {
            SpawnResult(r4);
        }
    }

    bool ValidPair(int previous, int next)
    {
        if (previous == next)
            return true;
        
        if ((previous + 1) % 4 == next)
            return true;

        return false;
    }

    void SpawnResult(int finalRotation)
    {
        Instantiate(resultPrefabs[finalRotation], spawnPoint.position, Quaternion.identity);
        cauldron.Brew();
    }

    public int GetCurrentState()
    {
        return cube4.GetRotationIndex();
    }
}
