using System.Collections.Generic;
using UnityEngine;

public class CauldronInventory : MonoBehaviour, IUniversalInteractable
{
    [Header("Recipes")]
    public List<PotionRecipe> potionRecipes = new List<PotionRecipe>();

    [Header("References")]
    public TemperatureManager tempManger;
    public Transform potionSpawnPosition;
    public CauldronWater waterCauldron;
    public CauldronWater waterBucket;

    private List<string> currentIngredients = new List<string>();
    private bool hasWater;

    [Header("VFX")]
    public GameObject spillVFX;
    public GameObject bottleFillVFX;
    public GameObject vfxSuccess;
    public GameObject vfxFail;
    public Transform caulVFXSpawnPoint;
    public Transform waterVFXSpawnPoint;

    [Header("Bottle")]
    public Transform bottleSnapPoint;
    public BottleSlot bottleSlot;
    public PickupItem currentBottle;

    [Header("Ingredient Visual")]
    public Transform ingredientDropPoint;
    public float destroyVisualAfter = 2f;

    public bool ShowInteractionPrompt => true;

    public void Interact(PlayerInteractionContext context) => TryAddIngredient(context);

    public void AlternateInteract(PlayerInteractionContext context) { }

    private void TryAddIngredient(PlayerInteractionContext context)
    {
        PickupItem selectedItem = context.SelectedItem;

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

        if (selectedItem.itemType == PickupItem.ItemType.Bucket && selectedItem.containedLiquid == PickupItem.LiquidType.Water)
        {
            if (AddWater())
            {
                selectedItem.containedLiquid = PickupItem.LiquidType.None;
            }

            return;
        }

        if (selectedItem.itemType != PickupItem.ItemType.Ingredient)
        {
            Debug.Log("Only ingredients can be added to the cauldron!");
            return;
        }


        SpawnIngredientVisual(selectedItem);
        AddIngredient(selectedItem.itemName);
        context.Inventory.RemoveSelected();

        Debug.Log(selectedItem.itemName + " added to cauldron.");
    }

    public void AddIngredient(string itemName)
    {
        currentIngredients.Add(itemName);
        Debug.Log("Added ingredient: " + itemName);
    }

    public void Brew(PhysicalState currentState)
    {
        PotionState currentTemperature = tempManger.finalTemperature;

        Debug.Log(
            $"BREW CHECK | State: {currentState} | Temperature: {currentTemperature}"
        );

        foreach (PotionRecipe recipe in potionRecipes)
        {
            bool ingredientsMatch = IsRecipeMatch(recipe);
            bool stateMatch = recipe.requiredPhysicalState == currentState;
            bool tempMatch = recipe.requiredTemperature == currentTemperature;

            Debug.Log(
                $"Recipe: {recipe.recipeName} | " +
                $"Ingredients: {ingredientsMatch} | " +
                $"State: {stateMatch} " +
                $"(Required: {recipe.requiredPhysicalState}, Current: {currentState}) | " +
                $"Temperature: {tempMatch} " +
                $"(Required: {recipe.requiredTemperature}, Current: {currentTemperature})"
            );

            if (ingredientsMatch && stateMatch && tempMatch)
            {
                Debug.Log("Potion created: " + recipe.recipeName);

                VFXSuccess(caulVFXSpawnPoint.position);

                if (currentBottle == null)
                {
                    Debug.Log("No bottle in cauldron. Potion spilled.");

                    PlaySpillVFX(waterVFXSpawnPoint.position);
                    ResetCauldron();
                    return;
                }

                CreatePotionInBottle(recipe);
                ResetCauldron();
                return;
            }
        }

        Debug.Log("Potion failed!");

        VFXFail(caulVFXSpawnPoint.position);
        ResetCauldron();
    }

    private bool IsRecipeMatch(PotionRecipe recipe)
    {
        // Every ingredient required by the recipe must exist
        // at least once in the cauldron.
        foreach (string requiredIngredient in recipe.ingredients)
        {
            if (!currentIngredients.Contains(requiredIngredient))
                return false;
        }

        return true;
    }

    private bool AddWater()
    {
        if (hasWater)
        {
            Debug.Log("Cauldron is already full of water!");
            return false;
        }

        hasWater = true;

        waterCauldron.FillWater();
        waterBucket.EmptyWater();

        Debug.Log("Water added to cauldron!");

        return true;
    }

    private void CreatePotionInBottle(PotionRecipe recipe)
    {
        if (currentBottle == null)
            return;

        PotionItem potion =
            currentBottle.GetComponent<PotionItem>();

        if (potion == null)
            potion =
                currentBottle.gameObject.AddComponent<PotionItem>();

        potion.potionID = recipe.recipeName;
        potion.targetAnimalID = recipe.targetAnimalID;

        currentBottle.itemType =
            PickupItem.ItemType.Potion;

        currentBottle.itemName =
            recipe.recipeName;

        PlayBottleFillVFX(
            waterVFXSpawnPoint.position
        );

        Debug.Log(
            "Bottle filled with " +
            recipe.recipeName
        );
    }

    private void ResetCauldron()
    {
        currentIngredients.Clear();
        hasWater = false;
        waterCauldron.EmptyWater();
    }

    public void VFXSuccess(Vector3 position)
    {
        GameObject vfx = Instantiate(vfxSuccess, position, Quaternion.identity);
        Destroy(vfx, 3f);
    }

    public void VFXFail(Vector3 position)
    {
        GameObject vfx = Instantiate(vfxFail, position, Quaternion.identity);
        Destroy(vfx, 3f);
    }

    private void SpawnIngredientVisual(PickupItem originalItem)
    {
        if (originalItem == null) return;

        GameObject visual = Instantiate(originalItem.gameObject, ingredientDropPoint.position, Random.rotation);

        foreach (Renderer r in visual.GetComponentsInChildren<Renderer>())
            r.enabled = true;

        foreach (Collider c in visual.GetComponentsInChildren<Collider>())
            c.enabled = true;

        Rigidbody rb = visual.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        PickupItem pickup = visual.GetComponent<PickupItem>();
        if (pickup != null) Destroy(pickup);

        Destroy(visual, destroyVisualAfter);
    }

    public void PlaySpillVFX(Vector3 position)
    {
        GameObject vfx = Instantiate(spillVFX, position, Quaternion.identity);
        Destroy(vfx, 3f);
    }

    public void PlayBottleFillVFX(Vector3 position)
    {
        GameObject vfx = Instantiate(bottleFillVFX, position, Quaternion.identity);
        Destroy(vfx, 3f);
    }

    public void SnapBottle(PickupItem bottle, Vector3 position)
    {
        if (bottle == null)
            return;

        if (bottle.itemType != PickupItem.ItemType.EmptyBottle &&
            bottle.itemType != PickupItem.ItemType.Potion)
        {
            return;
        }

        if (currentBottle != null)
            return;

        currentBottle = bottle;

        bottle.SetCauldron(this);

        if (bottle.rb != null)
        {
            bottle.rb.linearVelocity = Vector3.zero;
            bottle.rb.angularVelocity = Vector3.zero;

            bottle.rb.isKinematic = true;
            bottle.rb.useGravity = false;
        }

        bottle.transform.position = position;
        bottle.transform.rotation =
            Quaternion.Euler(-90f, 0f, 0f);
    }

    public void RemoveItem(PickupItem bottle)
    {
        if (bottle == null)
            return;

        if (bottle != currentBottle)
            return;

        if (bottle.rb != null)
        {
            bottle.rb.isKinematic = false;
            bottle.rb.useGravity = true;

            bottle.rb.linearVelocity = Vector3.zero;
            bottle.rb.angularVelocity = Vector3.zero;
        }

        bottle.SetCauldron(null);

        currentBottle = null;

        if (bottleSlot != null)
            bottleSlot.EnableSlot();

        Debug.Log("Bottle/potion removed from cauldron slot.");
    }

    public bool IsCurrentBottle(PickupItem bottle) => bottle == currentBottle;
}