using UnityEngine;
using System.Collections;
public class RotateHandle : MonoBehaviour
{
    private bool isRotating = false;
    public float rotationDuration = 1f;
    private int rotationIndex = 0;
    
    public void Rotate()
    {
        if (isRotating)
            return;

        StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;

        rotationIndex = (rotationIndex + 3) % 4;

        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, 180f);

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
    
}
