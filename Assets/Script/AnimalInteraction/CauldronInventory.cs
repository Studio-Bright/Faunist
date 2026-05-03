using System.Collections.Generic;
using UnityEngine;

public class CauldronInventory : MonoBehaviour, IInteractable
{
    public List<PotionRecipe> potionRecipes = new List<PotionRecipe>();
    private List<string> currentIngredients = new List<string>();

    public PotionTemperatureManager temperatureManager;
    public Transform PotionSpawnPosition;


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

        if (!hasWater && selectedItem.itemType == PickupItem.ItemType.Ingredient)
        {
            Debug.Log("You need water first!");
            return;
        }

        if (selectedItem.itemType == PickupItem.ItemType.Bucket &&
            selectedItem.containedLiquid == PickupItem.LiquidType.Water)
        {
            AddWater();
            selectedItem.containedLiquid = PickupItem.LiquidType.None;
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
    public void Brew(PhysicalState currentState)
    {

        if (!temperatureManager.temperatureReady)
        {
            Debug.Log("Temperature not set!");
            return;
        }

        PotionState currentTemperature = temperatureManager.finalTemperature;

        foreach (var recipe in potionRecipes)
        {
            if (IsRecipeMatch(recipe) &&
                recipe.requiredPhysicalState == currentState &&
                recipe.requiredTemperature == currentTemperature)
            {
                Debug.Log("✅ Potion created: " + recipe.recipeName);

                Instantiate(recipe.potionPrefab, PotionSpawnPosition.position, Quaternion.identity);

                ResetCauldron();
                return;
            }
        }

        Debug.Log("❌ Potion failed!");
        ResetCauldron();
    }

    private void ResetCauldron()
    {
        currentIngredients.Clear();
        temperatureManager.temperatureReady = false;
        hasWater = false;
    }

    private bool IsRecipeMatch(PotionRecipe recipe)
    {
        if (recipe.ingredients.Count != currentIngredients.Count)
            return false;

        List<string> temp = new List<string>(currentIngredients);

        foreach (var required in recipe.ingredients)
        {
            if (!temp.Contains(required))
                return false;

            temp.Remove(required);
        }

        return temp.Count == 0;
    }

    private void AddWater()
    {
        if (hasWater)
        {
            Debug.Log("Cauldron already has water!");
            return;
        }

        hasWater = true;
        Debug.Log("💧 Water added to cauldron!");
    }
}