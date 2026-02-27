using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public List<Node> connectedNodes = new List<Node>();

    [HideInInspector] public bool isStart;
    [HideInInspector] public bool isEnd;

    public bool IsConnectedTo(Node other)
    {
        return connectedNodes.Contains(other);
    }
}
