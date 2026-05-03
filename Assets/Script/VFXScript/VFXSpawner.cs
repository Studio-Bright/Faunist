using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    public GameObject boomVFX; // assign prefab in Inspector

    public void PlayBoom(Vector3 position)
    {
        GameObject vfx = Instantiate(boomVFX, position, Quaternion.identity);

        // Optional: destroy after duration
        Destroy(vfx, 3f);
    }
}