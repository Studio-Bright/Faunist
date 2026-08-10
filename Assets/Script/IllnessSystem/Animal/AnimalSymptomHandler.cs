using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AnimalSymptomHandler : MonoBehaviour
{
    [SerializeField] private AnimalDefinition definition;
    [SerializeField] private AnimalAttachmentPoints attachmentPoints;
    [SerializeField] private AnimalVisuals visuals;
    private readonly List<GameObject> spawnedVFX = new();

    public void ApplyIllness(IllnessData illness)
    {
        ClearSymptoms();

        foreach (var symptom in illness.symptoms)
        {
            ApplySymptom(symptom);
        }
    }

    private void ClearSymptoms()
    {
        foreach (GameObject obj in spawnedVFX)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedVFX.Clear();
    }

    private void ApplySymptom(SymptomData symptom)
    {
        AnimalSymptomSetup setup = definition.GetSetup(symptom);

        if (setup == null)
            return;

        if (!setup.vfx.useVFX)
            return;

        if (setup.vfx.vfxPrefab == null)
            return;

        Transform point = attachmentPoints.GetPoint(setup.vfx.spawnPoint);

        if (point == null)
        {
            Debug.LogWarning($"{name} is missing the {setup.vfx.spawnPoint} attachment point.");
            return;
        }

        GameObject obj = Instantiate(
            setup.vfx.vfxPrefab,
            point.position,
            point.rotation,
            point);

        spawnedVFX.Add(obj);

        VisualEffect vfx = obj.GetComponent<VisualEffect>();

        if (vfx != null && setup.vfx.requiresBodyMesh)
        {
            vfx.SetSkinnedMeshRenderer(
                "Skinned mesh",
                visuals.bodyMesh);
        }
    }
}