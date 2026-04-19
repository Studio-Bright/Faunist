using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PotionRecipe", menuName = "Potions/Recipe")]
public class PotionRecipe : ScriptableObject
{
    public string recipeName;

    public List<string> ingredients;

    public PhysicalState requiredPhysicalState;

    public PotionState requiredTemperature;

    public GameObject potionPrefab;
}