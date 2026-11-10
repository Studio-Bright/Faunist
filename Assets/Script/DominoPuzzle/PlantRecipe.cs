using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantRecipe", menuName = "Plants/Plant Recipe")]
public class PlantRecipe : ScriptableObject
{
    [Header("Plant Info")]
    public string plantName;
    public GameObject plantPrefab;

    [Header("Rune Chain Requirement")]
    public int firstRune;
    public int lastRune;
}