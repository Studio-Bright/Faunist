using UnityEngine;
using System.Collections;

public class RotatableItem : MonoBehaviour, IUniversalInteractable
{
    public float rotationDuration = 0.5f;

    private int rotationIndex;
    private bool isRotating;

    public bool ShowInteractionPrompt => true;

    public void Interact(PlayerInteractionContext context)
    {
        RotateSelf();
    }

    public void AlternateInteract(PlayerInteractionContext context)
    {
    }

    public int GetRotationIndex()
    {
        return rotationIndex;
    }

    public bool IsRotating()
    {
        return isRotating;
    }

    public void RotateSelf()
    {
        if (isRotating)
            return;

        if (!IsAligned())
            return;

        StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation =
            startRotation * Quaternion.Euler(0f, 0f, 90f);

        rotationIndex = (rotationIndex + 1) % 4;

        Debug.Log($"{name} -> rotation index: {rotationIndex}");

        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(
                startRotation,
                endRotation,
                elapsed / rotationDuration
            );

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.rotation = endRotation;

        isRotating = false;

    }

    private bool IsAligned()
    {
        float z = transform.localEulerAngles.z;
        float remainder = z % 90f;

        return Mathf.Abs(remainder) < 0.1f ||
               Mathf.Abs(remainder - 90f) < 0.1f;
    }
}