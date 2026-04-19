using UnityEngine;

public class Drawer : MonoBehaviour
{
    public Transform drawerTransform;
    public Vector3 openOffset = new Vector3(0, 0, 0.3f);
    public float openSpeed = 5f;

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

    public void Toggle()
    {
        isOpen = !isOpen;
    }

    void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;
        drawerTransform.localPosition = Vector3.Lerp(
            drawerTransform.localPosition,
            target,
            Time.deltaTime * openSpeed
        );
    }
}