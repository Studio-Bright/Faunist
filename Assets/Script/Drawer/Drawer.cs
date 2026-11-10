using UnityEngine;

public class Drawer : MonoBehaviour, IUniversalInteractable
{
    public Transform drawerTransform;
    public Vector3 openOffset = new Vector3(0, 0, 0.3f);
    public float openSpeed = 5f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen;

    public bool ShowInteractionPrompt => true;

    void Start()
    {
        if (drawerTransform == null) drawerTransform = transform;

        closedPos = drawerTransform.localPosition;
        openPos = closedPos + openOffset;
    }

    public void Interact(PlayerInteractionContext context) => Toggle();

    public void AlternateInteract(PlayerInteractionContext context) { }

    public void Toggle()
    {
        isOpen = !isOpen;
     
    }

    void Update()
    {
        if (drawerTransform == null) return;

        Vector3 target = isOpen ? openPos : closedPos;
        drawerTransform.localPosition = Vector3.Lerp(drawerTransform.localPosition, target, Time.deltaTime * openSpeed);
    }
}
