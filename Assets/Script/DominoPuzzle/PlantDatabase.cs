using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantDatabase", menuName = "Domino Puzzle/Plant Database")]
public class PlantDatabase : ScriptableObject
{
    public List<PlantRecipe> plants = new List<PlantRecipe>();

    /// <summary>
    /// Searches the database for a plant that matches the provided start and end rune values.
    /// </summary>
    public PlantRecipe GetMatchingPlant(int startValue, int endValue)
    {
        foreach (PlantRecipe plant in plants)
        {
            if (plant != null && plant.firstRune == startValue && plant.lastRune == endValue)
            {
                return plant;
            }
        }
        return null; // No recipe matches this combination
    }
}