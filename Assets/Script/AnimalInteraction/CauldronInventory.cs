using System.Collections.Generic;
using UnityEngine;

public class CauldronInventory : MonoBehaviour, IInteractable
{
    public List<PotionRecipe> potionRecipes = new List<PotionRecipe>();
    private List<string> currentIngredients = new List<string>();

    public PhysicalStatePuzzle physicalPuzzle;
    public PotionTemperatureManager temperatureManager;

    public void Interact(PlayerInteraction player)
    {
        TryAddIngredient(player);
    }

   

    // =========================
    // ADD INGREDIENT
    // =========================

    private void TryAddIngredient(PlayerInteraction player)
    {
        PickupItem selectedItem = player.inventory.GetSelectedItem();

        if (selectedItem == null)
        {
            Debug.Log("No item selected!");
            return;
        }

        AddIngredient(selectedItem.itemName);

        player.inventory.RemoveSelected();

        Debug.Log(selectedItem.itemName + " added to cauldron.");
    }

    public void AddIngredient(string itemName)
    {
        currentIngredients.Add(itemName);
        Debug.Log("Added to cauldron: " + itemName);
    }

    // =========================
    // BREW
    // =========================
    public void Brew()
    {
        if (!temperatureManager.temperatureReady)
        {
            Debug.Log("Temperature not set!");
            return;
        }

        int currentState = physicalPuzzle.GetCurrentState();
        var currentTemperature = temperatureManager.finalTemperature;

        foreach (var recipe in potionRecipes)
        {
            if (IsRecipeMatch(recipe) &&
                recipe.requiredPhysicalState == currentState &&
                recipe.requiredTemperature == currentTemperature)
            {
                Debug.Log("✅ Potion created: " + recipe.recipeName);

                currentIngredients.Clear();
                temperatureManager.temperatureReady = false;
                return;
            }
        }

        Debug.Log("❌ Potion failed!");

        currentIngredients.Clear();
        temperatureManager.temperatureReady = false;
    }
    // =========================
    // MATCH CHECK
    // =========================
    private bool IsRecipeMatch(PotionRecipe recipe)
    {
        // must match count exactly
        if (recipe.ingredients.Count != currentIngredients.Count)
            return false;

        List<string> temp = new List<string>(currentIngredients);

        foreach (var required in recipe.ingredients)
        {
            if (!temp.Contains(required))
                return false;

            temp.Remove(required);
        }

        // extra items protection
        return temp.Count == 0;
    }
}