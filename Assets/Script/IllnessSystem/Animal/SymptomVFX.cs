using UnityEngine;

[System.Serializable]
public class SymptomVFX
{
    public bool useVFX;

    public GameObject vfxPrefab;

    public AttachmentPointType spawnPoint;

    public bool requiresBodyMesh;
}