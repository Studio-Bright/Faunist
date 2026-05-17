using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [System.Serializable]
    public class VFXData
    {
        public string vfxName;

        [Header("Prefab")]
        public GameObject prefab;

        [Header("Transform Offsets")]
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        public Vector3 scale = Vector3.one;

        [Header("Lifetime")]
        public bool destroyAutomatically = true;
        public float destroyAfter = 3f;
    }

    [Header("All Effects")]
    public List<VFXData> allVFX = new List<VFXData>();

    private Dictionary<string, VFXData> vfxDict;

    private void Awake()
    {
        Instance = this;

        vfxDict = new Dictionary<string, VFXData>();

        foreach (VFXData vfx in allVFX)
        {
            if (!vfxDict.ContainsKey(vfx.vfxName))
            {
                vfxDict.Add(vfx.vfxName, vfx);
            }
        }
    }

    // Main Function
    public GameObject PlayVFX(string vfxName, Transform point)
    {
        if (!vfxDict.ContainsKey(vfxName))
        {
            Debug.LogWarning("No VFX named: " + vfxName);
            return null;
        }

        VFXData data = vfxDict[vfxName];

        Vector3 spawnPos =
            point.position + point.TransformDirection(data.positionOffset);

        Quaternion spawnRot =
            point.rotation * Quaternion.Euler(data.rotationOffset);

        GameObject spawned =
            Instantiate(data.prefab, spawnPos, spawnRot);

        spawned.transform.localScale = data.scale;

        if (data.destroyAutomatically)
        {
            Destroy(spawned, data.destroyAfter);
        }

        return spawned;
    }

    // Optional manual destroy helper
    public void StopVFX(GameObject vfxObject)
    {
        if (vfxObject != null)
        {
            Destroy(vfxObject);
        }
    }
}