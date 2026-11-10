using UnityEngine;
using System.Collections;

public class RotateHandle : MonoBehaviour, IUniversalInteractable
{
    [Header("Rotation")]
    public float rotationDuration = 1f;

    [Header("Cooldown")]
    public float cooldownDuration = 3f;

    [Header("Puzzle")]
    [SerializeField] private PhysicalStatePuzzle puzzle;

    private bool isRotating;
    private bool isOnCooldown;

    private int rotationIndex;

    public bool ShowInteractionPrompt => true;

    public void Interact(PlayerInteractionContext context)
    {
        Rotate();
    }

    public void AlternateInteract(PlayerInteractionContext context)
    {
    }

    public void Rotate()
    {
        if (isRotating)
            return;

        if (isOnCooldown)
            return;

        StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;

        rotationIndex = (rotationIndex + 3) % 4;

        Quaternion endRotation =
            startRotation * Quaternion.Euler(0f, 0f, 180f);

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

        if (puzzle != null)
        {
            puzzle.CheckPuzzle();
        }

        isOnCooldown = true;

        yield return new WaitForSeconds(cooldownDuration);

        isOnCooldown = false;
    }
}