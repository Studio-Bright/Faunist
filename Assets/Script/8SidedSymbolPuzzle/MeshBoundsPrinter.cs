using UnityEngine;

public class MeshBoundsPrinter : MonoBehaviour
{
    void Start()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;

        Debug.Log("Min Y: " + mesh.bounds.min.y);
        Debug.Log("Max Y: " + mesh.bounds.max.y);
    }
}