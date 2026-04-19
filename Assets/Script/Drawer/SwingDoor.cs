using UnityEngine;

public class SwingDoor : MonoBehaviour
{
    public Transform hinge; // child object marking hinge position
    public float openAngle = 90f;
    public float speed = 120f; // degrees per second

    private bool isOpen;
    private float currentAngle = 0f;

    public void Toggle()
    {
        isOpen = !isOpen;
    }

    void Update()
    {
        if (hinge == null) return;

        float targetAngle = isOpen ? openAngle : 0f;

        float angle = Mathf.MoveTowards(currentAngle, targetAngle, speed * Time.deltaTime);
        float delta = angle - currentAngle;

        // Rotate around hinge point (world space)
        transform.RotateAround(hinge.position, Vector3.up, delta);

        currentAngle = angle;
    }
}