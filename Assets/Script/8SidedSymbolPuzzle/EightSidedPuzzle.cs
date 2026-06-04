using System.Collections.Generic;
using UnityEngine;

public class EightSidedPuzzle : MonoBehaviour
{
    [Header("Setup")]
    public Camera puzzleCamera;

    [Header("External")]
    public PotionTemperatureManager temperatureManager;

    [Header("Nodes")]
    [SerializeField] private List<Node> outerNodes = new List<Node>();
    [SerializeField] private Node centerNode;
    [SerializeField] private Node startNode;
    [SerializeField] private Node endNode;

    private List<Node> currentPath = new List<Node>();
    private Node currentNode;

    private bool isDrawing = false;
    private bool isCompleted = false;
    public bool isInteractable = false;

    private PathSegment[] segments;
    private Dictionary<(int, int), PathSegment> segmentMapRef;

    // Internal record of all nodes belonging to THIS specific puzzle instance
    private HashSet<Node> myNodes = new HashSet<Node>();

    void Start()
    {
        segments = GetComponentsInChildren<PathSegment>(true);

        // Track every Node component under this specific object hierarchy
        foreach (var node in GetComponentsInChildren<Node>(true))
        {
            myNodes.Add(node);
        }

        // SAFETY FALLBACK: If lists weren't manual assigned, populate them from children
        if (outerNodes == null || outerNodes.Count == 0)
        {
            Debug.LogWarning($"[EightSidedPuzzle] Outer Nodes list on {gameObject.name} was empty! Populating automatically from children.");
            foreach (var node in myNodes)
            {
                if (node == centerNode || node == startNode || node == endNode) continue;
                outerNodes.Add(node);
            }
        }

        BuildSegmentMap();
        ConnectNodes();

        if (temperatureManager != null)
        {
            temperatureManager.RegisterPuzzle(this);
        }
        else
        {
            Debug.LogError($"[EightSidedPuzzle] PotionTemperatureManager reference is missing on {gameObject.name}!");
        }
    }

    void Update()
    {
        if (!isInteractable || isCompleted)
            return;

        bool leftClick = Input.GetMouseButtonDown(0);
        bool leftHold = Input.GetMouseButton(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        // Handle path cancellations instantly
        if (rightClick && isDrawing)
        {
            ClearPath();
            isDrawing = false;
            return;
        }

        // Cancel the path if the player releases the mouse click before reaching the end
        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            ClearPath();
            isDrawing = false;
            return;
        }

        if (!leftClick && !leftHold)
            return;

        Node hitNode = RaycastNode();

        // Ensure the clicked node belongs strictly to this puzzle block instance
        if (hitNode == null || !myNodes.Contains(hitNode))
            return;

        if (leftClick && !isDrawing)
        {
            TryStartPath(hitNode);
        }
        else if (leftHold && isDrawing)
        {
            TryExtendPath(hitNode);
        }
    }

    #region Setup

    void ConnectNodes()
    {
        if (centerNode == null) return;

        foreach (var node in outerNodes)
        {
            if (node != null) node.connectedNodes.Clear();
        }

        centerNode.connectedNodes.Clear();

        for (int i = 0; i < outerNodes.Count; i++)
        {
            Node current = outerNodes[i];
            if (current == null) continue;

            Node next = outerNodes[(i + 1) % outerNodes.Count];
            Node prev = outerNodes[(i - 1 + outerNodes.Count) % outerNodes.Count];

            if (!current.connectedNodes.Contains(centerNode)) current.connectedNodes.Add(centerNode);
            if (!centerNode.connectedNodes.Contains(current)) centerNode.connectedNodes.Add(current);

            if (next != null && !current.connectedNodes.Contains(next)) current.connectedNodes.Add(next);
            if (prev != null && !current.connectedNodes.Contains(prev)) current.connectedNodes.Add(prev);
        }
    }

    #endregion

    #region Drawing

    void TryStartPath(Node hitNode)
    {
        if (hitNode == startNode)
        {
            isDrawing = true;
            currentPath.Clear();

            currentNode = hitNode;
            currentPath.Add(hitNode);

            hitNode.SetGlow(true);
        }
    }

    void TryExtendPath(Node hitNode)
    {
        if (hitNode == currentNode) return;

        if (currentNode.IsConnectedTo(hitNode) && !currentPath.Contains(hitNode))
        {
            ActivateSegment(currentNode, hitNode);

            currentNode = hitNode;
            currentPath.Add(hitNode);

            hitNode.SetGlow(true);

            if (currentNode == endNode)
                CompletePuzzle();
        }
    }

    void CompletePuzzle()
    {
        isDrawing = false;
        isCompleted = true;

        int segmentCount = currentPath.Count - 1;
        int tempValue = CalculateTemperatureValue(segmentCount);

        Debug.Log($"[Puzzle Complete] {gameObject.name} finished with {segmentCount} segments. Sending Value: {tempValue}");
        temperatureManager.RegisterResult(tempValue);
    }

    #endregion

    #region Helpers

    Node RaycastNode()
    {
        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.collider.GetComponentInParent<Node>();
        }

        return null;
    }

    void ClearPath()
    {
        currentPath.Clear();

        foreach (var seg in segments)
            seg.SetActive(false);

        foreach (var node in myNodes)
            node.SetGlow(false);
    }

    public void ResetPuzzle()
    {
        isCompleted = false;
        isDrawing = false;
        isInteractable = true; // Stay interactable if player is still working on the mechanism station
        currentPath.Clear();

        foreach (var seg in segments)
            seg.SetActive(false);

        foreach (var node in myNodes)
            node.SetGlow(false);
    }

    int CalculateTemperatureValue(int segments)
    {
        if (segments <= 2) return -2;
        if (segments <= 4) return -1;
        if (segments <= 6) return 1;
        return 2;
    }

    #endregion

    #region Segment Handling

    void ActivateSegment(Node a, Node b)
    {
        int idA = a.GetInstanceID();
        int idB = b.GetInstanceID();

        if (segmentMapRef.TryGetValue((idA, idB), out PathSegment segment))
        {
            segment.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"[Puzzle Error] No segment found between {a.name} and {b.name}.");
        }
    }

    void BuildSegmentMap()
    {
        segmentMapRef = new Dictionary<(int, int), PathSegment>();

        foreach (var seg in segments)
        {
            if (seg.nodeA == null || seg.nodeB == null) continue;

            int idA = seg.nodeA.GetInstanceID();
            int idB = seg.nodeB.GetInstanceID();

            segmentMapRef[(idA, idB)] = seg;
            segmentMapRef[(idB, idA)] = seg;
        }
    }

    #endregion
}