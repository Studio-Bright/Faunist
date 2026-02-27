using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DominoPiece : MonoBehaviour
{
    [Header("Domino Values")]
    public int valueA;
    public int valueB;

    [Header("Pip Settings")]
    public GameObject pipPrefab;
    public Transform endA;
    public Transform endB;
    public float pipSpacing = 0.2f;

    [Header("Drag Settings")]
    public float yOffset = 0.05f;
    private bool hasSnappedOnce = false;
    private bool isSnappedToGrid = false;
    private Rigidbody rb;

    [HideInInspector] public DominoPuzzle parentPuzzle;

    // 🔹 STATIC drag controller (only one piece can drag at a time)
    private static DominoPiece currentlyDragging;
    private Vector3 dragOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();


        if (pipPrefab != null)
            GeneratePips();
    }

    void Update()
    {
        if (parentPuzzle == null || !parentPuzzle.isInteractable)
            return;

        Camera cam = Camera.main;

        // ===== ROTATE (RMB) =====
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                DominoPiece piece = hit.collider.GetComponent<DominoPiece>();
                if (piece != null)
                    piece.RotateSelf();
            }
        }

        // ===== START DRAG =====
        if (Input.GetMouseButtonDown(0) && currentlyDragging == null)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                DominoPiece piece = hit.collider.GetComponent<DominoPiece>();
                if (piece == this)
                {
                    currentlyDragging = this;
                }
            }
        }


        // ===== DRAGGING =====
        if (currentlyDragging == this && Input.GetMouseButton(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Board"))
                {
                    if (!hasSnappedOnce)
                    {
                        float snappedY = GetSnappedYRotation();

                        if (rb != null)
                            rb.MoveRotation(Quaternion.Euler(0f, snappedY, 0f));
                        else
                            transform.rotation = Quaternion.Euler(0f, snappedY, 0f);

                        hasSnappedOnce = true;

                        if (rb != null)
                        {
                            rb.isKinematic = true;
                            isSnappedToGrid = true;
                        }
                    }



                    Vector3 snapped = SnapToGrid(hit.point);

                    if (TryGetComponent<Rigidbody>(out var rb2))
                    {
                        rb2.MovePosition(new Vector3(
                            snapped.x,
                            hit.point.y + yOffset,
                            snapped.z));
                    }
                    else
                    {
                        transform.position = new Vector3(
                            snapped.x,
                            hit.point.y + yOffset,
                            snapped.z);
                    }

                }
            }
        }



        // ===== RELEASE =====
        if (currentlyDragging == this && Input.GetMouseButtonUp(0))
        {
            parentPuzzle.CheckPuzzle();
            currentlyDragging = null;
        }

    }

    Vector3 SnapToGrid(Vector3 worldPos)
    {
        float cellSize = parentPuzzle.cellSize;
        Vector3 origin = parentPuzzle.gridOrigin;

        float x = Mathf.Round((worldPos.x - origin.x) / cellSize) * cellSize;
        float z = Mathf.Round((worldPos.z - origin.z) / cellSize) * cellSize;

        Vector3 snapped = new Vector3(
            origin.x + x,
            worldPos.y,
            origin.z + z
        );

        if (IsHorizontal())
            snapped.x += cellSize * 0.5f;
        else
            snapped.z += cellSize * 0.5f;

        return snapped;
    }


    bool IsHorizontal()
    {
        float yRot = Mathf.Round(transform.eulerAngles.y);
        return yRot == 0f || yRot == 180f;
    }

    public void RotateSelf()
    {
        transform.Rotate(Vector3.up, 90f);
        parentPuzzle.CheckPuzzle();
    }

    public bool IsOnBoard()
    {
        // Cast a ray from slightly above the pivot downward
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            bool onBoard = hit.collider.CompareTag("Board");
            Debug.Log($"{name} ray hit: {hit.collider.name} at distance {hit.distance} → onBoard={onBoard}");
            return onBoard;
        }

        Debug.Log($"{name} ray missed the board!");
        return false;
    }


    public bool IsConnectedCorrectly()
    {
        float snapDistance = parentPuzzle.cellSize * 0.9f;
        DominoPiece[] allPieces = Object.FindObjectsByType<DominoPiece>(FindObjectsSortMode.None);
        int connections = 0;

        foreach (DominoPiece other in allPieces)
        {
            if (other == this) continue;

            bool connectedToThisPiece = false;

            Vector3 aA = FlattenY(endA.position);
            Vector3 aB = FlattenY(endB.position);
            Vector3 bA = FlattenY(other.endA.position);
            Vector3 bB = FlattenY(other.endB.position);

            float dAA = Vector3.Distance(aA, bA);
            float dAB = Vector3.Distance(aA, bB);
            float dBA = Vector3.Distance(aB, bA);
            float dBB = Vector3.Distance(aB, bB);

            Debug.Log($"{name} vs {other.name}: distances A↔A:{dAA:F2}, A↔B:{dAB:F2}, B↔A:{dBA:F2}, B↔B:{dBB:F2}");
            Debug.Log($"{name} values: A={valueA}, B={valueB} | {other.name} values: A={other.valueA}, B={other.valueB}");

            if (!connectedToThisPiece && dAA < snapDistance && valueA == other.valueA) connectedToThisPiece = true;
            if (!connectedToThisPiece && dAB < snapDistance && valueA == other.valueB) connectedToThisPiece = true;
            if (!connectedToThisPiece && dBA < snapDistance && valueB == other.valueA) connectedToThisPiece = true;
            if (!connectedToThisPiece && dBB < snapDistance && valueB == other.valueB) connectedToThisPiece = true;

            if (connectedToThisPiece)
            {
                connections++;
                Debug.Log($"{name} connected to {other.name}");
            }
        }

        Debug.Log($"{name} total connections: {connections}");

        if (connections == 0) return false;    // isolated
        if (connections >= 1) return true;     // valid chain
        return false;                       
    }




    Vector3 FlattenY(Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }



    public void GeneratePips()
    {
        if (pipPrefab == null || endA == null || endB == null)
            return;

        foreach (Transform child in endA) Destroy(child.gameObject);
        foreach (Transform child in endB) Destroy(child.gameObject);

        SpawnPips(endA, valueA);
        SpawnPips(endB, valueB);
    }

    void SpawnPips(Transform end, int value)
    {
        if (value <= 0) return;

        int rows = 2;
        int cols = Mathf.CeilToInt(value / 2f);
        int spawned = 0;

        for (int r = 0; r < rows && spawned < value; r++)
        {
            for (int c = 0; c < cols && spawned < value; c++)
            {
                Vector3 localPos = new Vector3(
                    (c - (cols - 1) * 0.5f) * pipSpacing,
                    0.01f,
                    (r - (rows - 1) * 0.5f) * pipSpacing
                );

                GameObject pip = Instantiate(pipPrefab, end);
                pip.transform.localPosition = localPos;
                spawned++;
            }
        }
    }

    float GetSnappedYRotation()
    {
        float y = transform.eulerAngles.y;
        float snapped = Mathf.Round(y / 90f) * 90f;
        return snapped;
    }


    bool IsCellOccupied(Vector3 targetPos)
    {
        float checkRadius = parentPuzzle.cellSize * 0.4f;

        Collider[] hits = Physics.OverlapSphere(
            targetPos + Vector3.up * 0.1f,
            checkRadius
        );

        foreach (var hit in hits)
        {
            DominoPiece other = hit.GetComponent<DominoPiece>();
            if (other != null && other != this)
                return true;
        }

        return false;
    }


    void OnDrawGizmosSelected()
    {
        if (parentPuzzle == null) return;

        float snapDistance = parentPuzzle.cellSize * 0.7f;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(endA.position, snapDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(endB.position, snapDistance);
    }

}
