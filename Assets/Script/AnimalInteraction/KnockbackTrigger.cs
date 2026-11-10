using UnityEngine;

public class KnockbackTrigger : MonoBehaviour
{
    public Transform explosionPoint;

    private PlayerMovementCC player;
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovementCC movement = other.GetComponent<PlayerMovementCC>();

        if (movement != null)
        {
            player = movement;
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovementCC movement = other.GetComponent<PlayerMovementCC>();

        if (movement != null && movement == player)
        {
            playerInside = false;
            player = null;
        }
    }

    public void Explode()
    {
        if (!playerInside || player == null)
            return;

        player.ApplyKnockback(explosionPoint.position);
    }
}