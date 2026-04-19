using UnityEngine;

public class SlidingDrawer : MonoBehaviour, IInteractable
{
    public Transform drawerTransform;
    public Vector3 openOffset = new Vector3(0, 0, 0.3f);
    public float speed = 5f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen;

    void Start()
    {
        if (drawerTransform == null)
            drawerTransform = transform;

        closedPos = drawerTransform.localPosition;
        openPos = closedPos + openOffset;
    }

    public void Interact(PlayerInteraction player)
    {
        isOpen = !isOpen;
    }

    void Update()
    {
        drawerTransform.localPosition = Vector3.Lerp(
            drawerTransform.localPosition,
            isOpen ? openPos : closedPos,
            Time.deltaTime * speed
        );
    }
}