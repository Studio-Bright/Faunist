using UnityEngine;

public class SwingDoor : MonoBehaviour, IUniversalInteractable
{
    public Transform hinge;
    public float openAngle = 90f;
    public float speed = 120f;

    private bool isOpen;
    private float currentAngle;

    public bool ShowInteractionPrompt => true;

    public void Interact(PlayerInteractionContext context) => Toggle();

    public void AlternateInteract(PlayerInteractionContext context) { }

    public void Toggle() => isOpen = !isOpen;

    void Update()
    {
        if (hinge == null) return;

        float targetAngle = isOpen ? openAngle : 0f;
        float angle = Mathf.MoveTowards(currentAngle, targetAngle, speed * Time.deltaTime);
        float delta = angle - currentAngle;

        transform.RotateAround(hinge.position, Vector3.up, delta);
        currentAngle = angle;
    }
}

