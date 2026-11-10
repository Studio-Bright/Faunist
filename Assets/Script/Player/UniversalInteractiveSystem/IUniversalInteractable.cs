public interface IUniversalInteractable
{
    bool ShowInteractionPrompt { get; }

    void Interact(PlayerInteractionContext context);

    void AlternateInteract(PlayerInteractionContext context);
}