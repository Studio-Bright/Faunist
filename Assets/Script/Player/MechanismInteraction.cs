using UnityEngine;
using System.Collections;

public class MechanismInteraction : MonoBehaviour, IInteractable
{
    public Transform cameraPoint; 
    public float transitionSpeed = 5f;

    public bool isActive = false;

    public void Interact(PlayerInteraction player)
    {
        if (isActive) return;

        player.StartCoroutine(EnterMechanism(player));
    }

    IEnumerator EnterMechanism(PlayerInteraction player)
    {
        isActive = true;

        player.DisablePlayerControl();

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

        isActive = false;
        player.EnablePlayerControl();
    }
}
