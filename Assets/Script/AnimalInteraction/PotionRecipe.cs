using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PotionRecipe
{
    public string recipeName;

    public List<string> ingredients = new List<string>();

    public int requiredPhysicalState;

    public PotionTemperatureManager.PotionState requiredTemperature;
}