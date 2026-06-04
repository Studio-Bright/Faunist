using System.Collections;
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
    public GameObject spillVFX;      // no bottle
    public GameObject bottleFillVFX; // bottle present
    public GameObject vfxSuccess;
    public GameObject vfxFail;
    public Transform caulVFXSpawnPoint;
    public Transform waterVFXSpawnPoint;

    [Header("Bottle")]
    public Transform bottleSnapPoint;
    public BottleSlot bottleSlot;
    private PickupItem currentBottle;


    [Header("Ingredient Visual")]
    public Transform ingredientDropPoint;
    public float destroyVisualAfter = 2f;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            VFXSuccess(caulVFXSpawnPoint.position);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayBottleFillVFX(waterVFXSpawnPoint.position);
        }
    }
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
        if (selectedItem.itemType != PickupItem.ItemType.Ingredient)
        {
            Debug.Log("Only ingredients can be added to the cauldron!");
            return;
        }
        // Add ingredient
        SpawnIngredientVisual(selectedItem);

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
            Debug.Log("Checking recipe: " + recipe.recipeName);

            bool ingredientsMatch = IsRecipeMatch(recipe);
            bool stateMatch = recipe.requiredPhysicalState == currentState;
            bool tempMatch = recipe.requiredTemperature == currentTemperature;

            Debug.Log(
                $"Ingredients={ingredientsMatch} | " +
                $"State={stateMatch} ({recipe.requiredPhysicalState} vs {currentState}) | " +
                $"Temp={tempMatch} ({recipe.requiredTemperature} vs {currentTemperature})"
            );

            if (ingredientsMatch &&
                stateMatch &&
                tempMatch)
            {
                Debug.Log("Potion created: " + recipe.recipeName);

                VFXSuccess(caulVFXSpawnPoint.position);

                if (currentBottle == null)
                {
                    Debug.Log("No bottle branch reached");

                    PlaySpillVFX(waterVFXSpawnPoint.position);

                    ResetCauldron();
                    return;
                }
                Debug.Log("Creating potion in bottle");
                CreatePotionInBottle(recipe);

                ResetCauldron();

                return;
            }
        }
        VFXFail(caulVFXSpawnPoint.position);
        Debug.Log("Potion failed!");

        ResetCauldron();
        temperatureManager.ResetAllPuzzles();
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
    private void CreatePotionInBottle(PotionRecipe recipe)
    {
        PotionItem potion = currentBottle.GetComponent<PotionItem>();

        if (potion == null)
        {
            potion = currentBottle.gameObject.AddComponent<PotionItem>();
        }

        potion.potionID =
            recipe.recipeName;

        potion.targetAnimalID =
            recipe.targetAnimalID;

        currentBottle.itemType =
            PickupItem.ItemType.Potion;

        currentBottle.itemName =
            recipe.recipeName;

        PlayBottleFillVFX(
            waterVFXSpawnPoint.transform.position);

        Debug.Log(
            $"Bottle filled with {recipe.recipeName}");
    }
    private void ResetCauldron()
    {
        currentIngredients.Clear();

        hasWater = false;

        temperatureManager.temperatureReady = false;

        waterCauldron.EmptyWater();
        
    }

    public void VFXSuccess(Vector3 position)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);

        GameObject vfx = Instantiate(vfxSuccess, position, rotation);

        Destroy(vfx, 3f);
    }
    public void VFXFail (Vector3 position)
    {
        GameObject vfx = Instantiate(vfxFail, position, Quaternion.identity);

        Destroy(vfx, 3f);
    }

    private void SpawnIngredientVisual(PickupItem originalItem)
    {
        if (originalItem == null)
            return;

        // Spawn visual copy
        GameObject visual =
            Instantiate(
                originalItem.gameObject,
                ingredientDropPoint.position,
                Random.rotation);

        // Make sure object is visible
        Renderer[] renderers =
            visual.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
            r.enabled = true;

        // Enable colliders
        Collider[] cols =
            visual.GetComponentsInChildren<Collider>();

        foreach (Collider c in cols)
            c.enabled = true;

        Rigidbody rb = visual.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Remove gameplay scripts
        PickupItem pickup =
            visual.GetComponent<PickupItem>();

        if (pickup != null)
            Destroy(pickup);

        Destroy(visual, destroyVisualAfter);
    }



    public void PlaySpillVFX(Vector3 position)
    {
        Debug.Log("PlaySpillVFX called");

        GameObject vfx =
            Instantiate(spillVFX, position, Quaternion.identity);

        Destroy(vfx, 3f);
    }

    public void PlayBottleFillVFX(Vector3 position)
    {
        Debug.Log("PlayBottleFillVFX called");

        GameObject vfx =
            Instantiate(bottleFillVFX, position, Quaternion.identity);

        Destroy(vfx, 3f);
    }

    public void SnapBottle(PickupItem bottle, Vector3 position)
    {
        if (bottle != null)
        {
            currentBottle = bottle;

            bottle.rb.linearVelocity = Vector3.zero;
            bottle.rb.angularVelocity = Vector3.zero;
            bottle.rb.isKinematic = true;
            bottle.rb.useGravity = false;

            bottle.transform.position = position;
            bottle.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
    }

    public void RemoveItem(PickupItem bottle)
    {
        PickupItem pickup = bottle.GetComponent<PickupItem>();
        if (pickup != null)
        {
            pickup.rb.isKinematic = false;
            pickup.rb.useGravity = true;
            pickup.rb.linearVelocity = Vector3.zero;
            pickup.rb.angularVelocity = Vector3.zero;
            bottleSlot.EnableSlot();

            currentBottle = null;
        }
        
    }
    public bool IsCurrentBottle(PickupItem bottle)
    {
        return bottle == currentBottle;
    }
}