using UnityEngine;

public class LadderFrontTrigger : MonoBehaviour
{
    public Ladder ladder;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovementCC player =
            other.GetComponent<PlayerMovementCC>();

        if (player != null)
        {
            player.SetLadderFrontArea(ladder, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovementCC player =
            other.GetComponent<PlayerMovementCC>();

        if (player != null)
        {
            player.SetLadderFrontArea(ladder, false);
        }
    }
}