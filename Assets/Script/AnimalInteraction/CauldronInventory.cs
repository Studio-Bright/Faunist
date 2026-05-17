using System.Collections.Generic;
using UnityEngine;

public class CauldronInventory : MonoBehaviour, IInteractable
{
    [Header("Recipes")]
    public List<PotionRecipe> potionRecipes = new List<PotionRecipe>();

    [Header("References")]
    public PotionTemperatureManager temperatureManager;
    public Transform potionSpawnPosition;
    public CauldronWater waterCauldron;
    public CauldronWater waterBucket;

    private List<string> currentIngredients = new List<string>();

    private bool hasWater = false;

    public void Interact(PlayerInteraction player)
    {
        TryAddIngredient(player);
    }

    private void TryAddIngredient(PlayerInteraction player)
    {
        PickupItem selectedItem = player.inventory.GetSelectedItem();

        if (selectedItem == null)
        {
            Debug.Log("No item selected!");
            return;
        }

        // Require water before ingredients
        if (!hasWater &&
            selectedItem.itemType == PickupItem.ItemType.Ingredient)
        {
            Debug.Log("You need water first!");
            return;
        }

        // Add water
        if (selectedItem.itemType == PickupItem.ItemType.Bucket &&
            selectedItem.containedLiquid == PickupItem.LiquidType.Water)
        {
            AddWater();

            selectedItem.containedLiquid =
                PickupItem.LiquidType.None;

            return;
        }

        // Add ingredient
        AddIngredient(selectedItem.itemName);

        player.inventory.RemoveSelected();

        Debug.Log(selectedItem.itemName + " added to cauldron.");
    }

    public void AddIngredient(string itemName)
    {
        currentIngredients.Add(itemName);

        Debug.Log("Added ingredient: " + itemName);
    }

    public void Brew(PhysicalState currentState)
    {
        if (!temperatureManager.temperatureReady)
        {
            Debug.Log("Temperature not set!");
            return;
        }

        PotionState currentTemperature =
            temperatureManager.finalTemperature;

        foreach (PotionRecipe recipe in potionRecipes)
        {
            if (IsRecipeMatch(recipe) &&
                recipe.requiredPhysicalState == currentState &&
                recipe.requiredTemperature == currentTemperature)
            {
                Debug.Log("Potion created: " + recipe.recipeName);

                Instantiate(
                    recipe.potionPrefab,
                    potionSpawnPosition.position,
                    Quaternion.identity
                );

                ResetCauldron();

                return;
            }
        }

        Debug.Log("Potion failed!");

        ResetCauldron();
    }

    private bool IsRecipeMatch(PotionRecipe recipe)
    {
        if (recipe.ingredients.Count != currentIngredients.Count)
            return false;

        List<string> tempIngredients =
            new List<string>(currentIngredients);

        foreach (string ingredient in recipe.ingredients)
        {
            if (!tempIngredients.Contains(ingredient))
                return false;

            tempIngredients.Remove(ingredient);
        }

        return tempIngredients.Count == 0;
    }

    private void AddWater()
    {
        if (hasWater)
        {
            Debug.Log("Cauldron already has water!");
            return;
        }

        hasWater = true;

        waterCauldron.FillWater();
        waterBucket.EmptyWater();

        Debug.Log("Water added to cauldron!");
    }

    private void ResetCauldron()
    {
        currentIngredients.Clear();

        hasWater = false;

        temperatureManager.temperatureReady = false;

        waterCauldron.EmptyWater();
        
    }
}