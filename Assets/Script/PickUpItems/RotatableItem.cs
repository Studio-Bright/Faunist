using UnityEngine;
using System.Collections;

public class RotatableItem : MonoBehaviour
{
    public float rotationDuration = 0.5f;

    private int rotationIndex = 0;
    private bool isRotating = false;

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
        // ❌ Prevent rotation spam
        if (isRotating)
            return;

        // ❌ Extra safety check (ensure aligned)
        if (!IsAligned())
            return;

        StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;

        rotationIndex = (rotationIndex + 3) % 4;

        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, 90f);

        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;

        isRotating = false;
    }

    private bool IsAligned()
    {
        float z = transform.localEulerAngles.z;

        // Snap tolerance check (handles floating point drift)
        float remainder = z % 90f;

        return Mathf.Abs(remainder) < 0.1f || Mathf.Abs(remainder - 90f) < 0.1f;
    }
}
