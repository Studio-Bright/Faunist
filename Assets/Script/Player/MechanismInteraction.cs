using UnityEngine;
using System.Collections;

public class MechanismInteraction : MonoBehaviour, IInteractable
{
    public Transform cameraPoint;
    public float transitionSpeed = 5f;

    public bool isActive = false;

    public PhysicalStatePuzzle puzzle;
    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
    }
    public void Interact(PlayerInteraction player)
    {
        if (isActive) return;

        player.StartCoroutine(EnterMechanism(player));
    }

    IEnumerator EnterMechanism(PlayerInteraction player)
    {
        isActive = true;

        if (col != null)
            col.enabled = false;

        player.DisablePlayerControl();

        if (puzzle != null)
            puzzle.SetInteraction(true);

        Transform cam = player.cam.transform;

        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        Vector3 targetPos = cameraPoint.position;
        Quaternion targetRot = cameraPoint.rotation;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            cam.position = Vector3.Lerp(startPos, targetPos, t);
            cam.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        player.EnablePuzzleMode(this);
    }

    public IEnumerator ExitMechanism(PlayerInteraction player)
    {
        Transform cam = player.cam.transform;

        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        Vector3 targetPos = player.originalCamPosition;
        Quaternion targetRot = player.originalCamRotation;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            cam.position = Vector3.Lerp(startPos, targetPos, t);
            cam.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        if (puzzle != null)
            puzzle.SetInteraction(false);

        if (col != null)
            col.enabled = true;

        isActive = false;
        player.EnablePlayerControl();
    }
}