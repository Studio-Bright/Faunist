using TMPro;
using UnityEngine;

public class Note : MonoBehaviour
{
    public TextMeshPro textMesh;

    [TextArea]
    public string noteText;

    void Start()
    {
        textMesh.text = noteText;
    }
}