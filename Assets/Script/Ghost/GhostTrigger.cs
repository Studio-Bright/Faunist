using UnityEngine;
using System.Collections;

public class GhostTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimalEncounterManager encounterManager;

    private bool playerInRange;
    private bool animationRunning;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Only allow ghost during snail levels
        if (encounterManager == null ||
            encounterManager.currentAnimal == null ||
            !encounterManager.currentAnimal.isSnailLevel)
            return;

        playerInRange = true;

        if (!animationRunning)
            StartCoroutine(GhostLoop());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
    }

    private IEnumerator GhostLoop()
    {
        animationRunning = true;

        while (playerInRange)
        {
            animator.Play("SoulAnimation", 0, 0f);

            yield return null;

            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                yield return null;
        }

        animationRunning = false;
    }
}   