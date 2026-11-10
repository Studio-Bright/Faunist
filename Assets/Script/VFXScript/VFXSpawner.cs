using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    public GameObject boomVFX;
    public KnockbackTrigger knockbackTrigger;

    public void PlayBoom(Vector3 position)
    {
        GameObject vfx = Instantiate(
            boomVFX,
            position,
            Quaternion.identity
        );

        Destroy(vfx, 3f);

        // Only knock back if player is currently inside the zone
        if (knockbackTrigger != null)
        {
            knockbackTrigger.Explode();
        }
    }
}