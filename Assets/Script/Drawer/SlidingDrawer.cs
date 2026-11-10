using UnityEngine;

public class SlidingDrawer : MonoBehaviour, IUniversalInteractable
{
    [Header("Drawer")]
    [SerializeField] private Transform drawerTransform;
    [SerializeField] private Vector3 openOffset = new Vector3(0f, 0f, 0.3f);
    [SerializeField] private float speed = 5f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen;

    public bool ShowInteractionPrompt => true;

    private void Start()
    {
        if (drawerTransform == null)
            drawerTransform = transform;

        closedPos = drawerTransform.localPosition;
        openPos = closedPos + openOffset;
    }

    public void Interact(PlayerInteractionContext context)
    {
        isOpen = !isOpen;
    }

    public void AlternateInteract(PlayerInteractionContext context) { }

    private void Update()
    {
        if (drawerTransform == null)
            return;

        drawerTransform.localPosition = Vector3.Lerp(drawerTransform.localPosition, isOpen ? openPos : closedPos, Time.deltaTime * speed);
    }
}