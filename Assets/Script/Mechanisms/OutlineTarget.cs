using UnityEngine;

public class OutlineTarget : MonoBehaviour
{
    private int originalLayer;
    private bool outlined = false;

    void Awake()
    {
        originalLayer = gameObject.layer;
    }

    public void SetOutline(bool enabled)
    {
        if (outlined == enabled)
            return;

        outlined = enabled;

        int targetLayer = enabled
            ? LayerMask.NameToLayer("Outline")
            : originalLayer;

        SetLayerRecursively(gameObject, targetLayer);
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}