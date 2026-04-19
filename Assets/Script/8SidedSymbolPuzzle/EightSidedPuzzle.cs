using System.Collections.Generic;
using UnityEngine;

public class EightSidedPuzzle : MonoBehaviour
{
    [Header("Setup")]
    public GameObject nodePrefab;
    public float radius = 3f;
    public float lineWidth = 0.05f;
    public Camera puzzleCamera;

    [Header("External")]
    public PotionTemperatureManager temperatureManager;

    private List<Node> outerNodes = new List<Node>();
    private Node centerNode;

    private Node startNode;
    private Node endNode;

    private List<Node> currentPath = new List<Node>();
    private Node currentNode;

    private LineRenderer lineRenderer;

    private bool isDrawing = false;
    private bool isCompleted = false;
    public bool isInteractable = false;


    void Start()
    {
        CreateNodes();
        ConnectNodes();
        SetupSpecialNodes();
        SetupLineRenderer();
        temperatureManager.RegisterPuzzle(this);

    }

    void Update()
    {
        if (isCompleted) return;

        if (Input.GetMouseButtonDown(0))
            TryStartPath();

        if (Input.GetMouseButton(0) && isDrawing)
            TryExtendPath();

        if (Input.GetMouseButtonUp(0))
            CancelIfIncomplete();

        if (!isInteractable) return;

    }

    #region Setup

    void CreateNodes()
    {
        GameObject centerObj = Instantiate(nodePrefab, transform);
        centerObj.transform.localPosition = Vector3.zero;
        centerNode = centerObj.GetComponent<Node>();

        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 2f / 8;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            GameObject nodeObj = Instantiate(nodePrefab, transform);
            nodeObj.transform.localPosition = pos;

            outerNodes.Add(nodeObj.GetComponent<Node>());
        }
    }

    void ConnectNodes()
    {
        for (int i = 0; i < outerNodes.Count; i++)
        {
            Node current = outerNodes[i];

            current.connectedNodes.Add(centerNode);
            centerNode.connectedNodes.Add(current);

            Node next = outerNodes[(i + 1) % outerNodes.Count];
            Node prev = outerNodes[(i - 1 + outerNodes.Count) % outerNodes.Count];

            current.connectedNodes.Add(next);
            current.connectedNodes.Add(prev);
        }
    }

    void SetupSpecialNodes()
    {
        float highest = float.MinValue;
        float lowest = float.MaxValue;

        foreach (Node node in outerNodes)
        {
            if (node.transform.position.z > highest)
            {
                highest = node.transform.position.z;
                startNode = node;
            }

            if (node.transform.position.z < lowest)
            {
                lowest = node.transform.position.z;
                endNode = node;
            }
        }
    }

    void SetupLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
    }

    #endregion

    #region Drawing

    void TryStartPath()
    {
        Node hitNode = RaycastNode();

        if (hitNode == startNode)
        {
            isDrawing = true;
            currentPath.Clear();
            currentNode = hitNode;
            currentPath.Add(hitNode);
            UpdateLine();
        }
    }

    void TryExtendPath()
    {
        Node hitNode = RaycastNode();

        if (hitNode == null) return;
        if (hitNode == currentNode) return;

        if (currentNode.IsConnectedTo(hitNode) && !currentPath.Contains(hitNode))
        {
            currentNode = hitNode;
            currentPath.Add(hitNode);
            UpdateLine();

            if (currentNode == endNode)
            {
                CompletePuzzle();
            }
        }
    }

    void CancelIfIncomplete()
    {
        if (!isDrawing) return;
        if (isCompleted) return;

        isDrawing = false;

        if (currentNode != endNode)
        {
            ClearPath();
        }
    }

    void CompletePuzzle()
    {
        isDrawing = false;
        isCompleted = true;

        int segments = currentPath.Count - 1;
        int tempValue = CalculateTemperatureValue(segments);

        temperatureManager.RegisterResult(tempValue);
    }

    #endregion

    #region Helpers

    Node RaycastNode()
    {
        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            return hit.collider.GetComponent<Node>();

        return null;
    }

    void UpdateLine()
    {
        lineRenderer.positionCount = currentPath.Count;

        for (int i = 0; i < currentPath.Count; i++)
            lineRenderer.SetPosition(i, currentPath[i].transform.position);
    }

    void ClearPath()
    {
        currentPath.Clear();
        lineRenderer.positionCount = 0;
    }

    int CalculateTemperatureValue(int segments)
    {
        if (segments <= 2) return -2;
        if (segments <= 4) return -1;
        if (segments <= 6) return +1;
        return +2;
    }

    public void ResetPuzzle()
    {
        isCompleted = false;
        isDrawing = false;
        currentPath.Clear();
        lineRenderer.positionCount = 0;
    }


    #endregion
}
