using UnityEngine;

public class Ladder : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;

    [Header("Front Climb Area")]
    public Collider frontClimbTrigger;

    public bool PlayerIsInFrontArea { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (frontClimbTrigger == null)
            return;

        if (other.GetComponent<PlayerMovementCC>() != null)
        {
            PlayerIsInFrontArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovementCC>() != null)
        {
            PlayerIsInFrontArea = false;
        }
    }
}