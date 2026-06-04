using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public List<Node> connectedNodes = new List<Node>();

    [HideInInspector] public bool isStart;
    [HideInInspector] public bool isEnd;
    public int id;
    public bool IsConnectedTo(Node other)
    {
        return connectedNodes.Contains(other);
    }

    [SerializeField] private Renderer rend;

    private Material mat;

    [SerializeField] private Color inactiveEmission = Color.black;
    [SerializeField] private Color activeEmission = new Color(5f, 4f, 0f);

    void Awake()
    {
        mat = rend.material;
        mat.EnableKeyword("_EMISSION");
        SetGlow(false);
        
    }

    public void SetGlow(bool state)
    {
        Color emission = state
            ? activeEmission
            : inactiveEmission;

        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", emission * 5f);
    }
}
