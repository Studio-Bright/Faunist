using UnityEngine;

public class PlayerInteractionContext
{
    public UniversalInteractionSystem Player { get; }

    public InventorySystem Inventory { get; }

    public PlayerHands Hands { get; }

    public PickupItem SelectedItem
    {
        get
        {
            return Inventory != null
                ? Inventory.GetSelectedItem()
                : null;
        }
    }

    public Camera Camera
    {
        get
        {
            return Player.PlayerCamera;
        }
    }

    public PlayerInteractionContext(
        UniversalInteractionSystem player,
        InventorySystem inventory,
        PlayerHands hands)
    {
        Player = player;
        Inventory = inventory;
        Hands = hands;
    }
}