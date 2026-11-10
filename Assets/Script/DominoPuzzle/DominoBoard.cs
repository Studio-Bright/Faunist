using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DominoBoard : MonoBehaviour
{
    [Header("Grid")]
    public int width = 5;
    public int height = 7;
    public float cellSize = 0.3f;

    [Header("Board Origin")]
    [Tooltip("The board GameObject should be positioned at the CENTER of the whole grid.")]
    public Vector3 boardLocalOffset = Vector3.zero;

    [Header("Grid Depth")]
    [Tooltip("Local Z position of the grid surface.")]
    public float gridDepth = 0f;

    [Header("Plant Puzzle Storage")]
    [Tooltip("Assign your PlantDatabase Scriptable Object here.")]
    public PlantDatabase plantDatabase;
    public Transform plantSpawnPoint;

    [Header("Gizmos")]
    public bool showStartSocket = true;
    public bool showEndSocket = true;

    private RunePiece[,] grid;
    private GameObject spawnedPlantInstance;

    // Dynamic runtime properties
    public PlantRecipe CurrentActivePlant { get; private set; }
    public int CurrentStartValue { get; private set; } = -1;
    public int CurrentEndValue { get; private set; } = -1;

    public Vector3 GridOriginLocal => boardLocalOffset + new Vector3(-((width - 1) * cellSize) / 2f, -((height - 1) * cellSize) / 2f, gridDepth);

    private void Awake() => grid = new RunePiece[width, height + 2];

    private int GetStorageY(int y) => y + 1;

    public bool IsInsideGrid(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;
    public bool IsStartCell(int x, int y) => x == width / 2 && y == -1;
    public bool IsEndCell(int x, int y) => x == width / 2 && y == height;
    public bool IsSpecialCell(int x, int y) => IsStartCell(x, y) || IsEndCell(x, y);
    public bool IsValidLogicalCell(int x, int y) => IsInsideGrid(x, y) || IsSpecialCell(x, y);

    public bool IsOccupied(int x, int y)
    {
        if (!IsValidLogicalCell(x, y)) return true;
        return grid[x, GetStorageY(y)] != null;
    }

    public RunePiece GetRunePiece(int x, int y)
    {
        if (!IsValidLogicalCell(x, y)) return null;
        return grid[x, GetStorageY(y)];
    }

    public bool CanPlaceDomino(int x, int y, bool horizontal)
    {
        int x2 = horizontal ? x + 1 : x;
        int y2 = horizontal ? y : y + 1;

        if (!IsValidLogicalCell(x, y) || !IsValidLogicalCell(x2, y2)) return false;
        if ((IsSpecialCell(x, y) || IsSpecialCell(x2, y2)) && horizontal) return false;

        if (IsStartCell(x, y) && y2 != 0) return false;
        if (IsEndCell(x2, y2) && y != height - 1) return false;

        if (IsStartCell(x2, y2) || IsEndCell(x, y)) return false;

        if (IsOccupied(x, y) || IsOccupied(x2, y2)) return false;

        return true;
    }

    public void PlaceDomino(int x, int y, bool horizontal, RunePiece rune)
    {
        int x2 = horizontal ? x + 1 : x;
        int y2 = horizontal ? y : y + 1;

        if (IsValidLogicalCell(x, y)) grid[x, GetStorageY(y)] = rune;
        if (IsValidLogicalCell(x2, y2)) grid[x2, GetStorageY(y2)] = rune;

        CheckBoardForGrowth();
    }

    public void RemoveDomino(int x, int y, bool horizontal)
    {
        int x2 = horizontal ? x + 1 : x;
        int y2 = horizontal ? y : y + 1;

        if (IsValidLogicalCell(x, y)) grid[x, GetStorageY(y)] = null;
        if (IsValidLogicalCell(x2, y2)) grid[x2, GetStorageY(y2)] = null;

        CheckBoardForGrowth();
    }

    public Vector3 GetCellCenter(int x, int y) => transform.TransformPoint(GridOriginLocal + new Vector3(x * cellSize, y * cellSize, 0f));
    public Vector3 GetStartPositionLocal() => GridOriginLocal + new Vector3((width / 2) * cellSize, -1 * cellSize, 0f);
    public Vector3 GetEndPositionLocal() => GridOriginLocal + new Vector3((width / 2) * cellSize, height * cellSize, 0f);
    public Vector3 GetStartPosition() => transform.TransformPoint(GetStartPositionLocal());
    public Vector3 GetEndPosition() => transform.TransformPoint(GetEndPositionLocal());

    public Vector3 GetDominoPosition(int x, int y, bool horizontal)
    {
        Vector3 cellA = GetCellCenter(x, y);
        Vector3 cellB = GetCellCenter(horizontal ? x + 1 : x, horizontal ? y : y + 1);
        return (cellA + cellB) * 0.5f;
    }

    public bool RaycastBoard(Ray ray, out Vector3 worldPoint)
    {
        Plane boardPlane = new Plane(transform.forward, transform.TransformPoint(new Vector3(boardLocalOffset.x, boardLocalOffset.y, gridDepth)));

        if (boardPlane.Raycast(ray, out float distance) && distance >= 0f)
        {
            worldPoint = ray.GetPoint(distance);
            return true;
        }

        worldPoint = Vector3.zero;
        return false;
    }

    public bool WorldToCell(Vector3 worldPosition, out int x, out int y)
    {
        Vector3 local = transform.InverseTransformPoint(worldPosition);
        Vector3 origin = GridOriginLocal;

        x = Mathf.RoundToInt((local.x - origin.x) / cellSize);
        y = Mathf.RoundToInt((local.y - origin.y) / cellSize);

        return IsValidLogicalCell(x, y);
    }

    public void CheckBoardForGrowth()
    {
        if (plantDatabase == null)
        {
            Debug.LogWarning("[DominoBoard] PlantDatabase is not assigned in the Inspector!");
            return;
        }

        int startX = width / 2;
        int startY = -1;
        int endX = width / 2;
        int endY = height;

        RunePiece startPiece = GetRunePiece(startX, startY);
        RunePiece endPiece = GetRunePiece(endX, endY);

        // 1. Both start and end sockets must have a piece placed
        if (startPiece == null || endPiece == null)
        {
            ClearActivePlant();
            return;
        }

        int detectedStartValue = startPiece.GetValueAtCell(startX, startY);
        int detectedEndValue = endPiece.GetValueAtCell(endX, endY);

        // 2. Check if sockets changed and look up recipe dynamically
        if (detectedStartValue != CurrentStartValue || detectedEndValue != CurrentEndValue)
        {
            CurrentStartValue = detectedStartValue;
            CurrentEndValue = detectedEndValue;
            CurrentActivePlant = plantDatabase.GetMatchingPlant(CurrentStartValue, CurrentEndValue);

            if (CurrentActivePlant != null)
            {
                Debug.Log($"[Database] Matching recipe found: {CurrentActivePlant.plantName} (Start: {CurrentStartValue}, End: {CurrentEndValue})");
            }
            else
            {
                Debug.Log($"[Database] No recipe exists for Start: {CurrentStartValue} -> End: {CurrentEndValue}");
                ClearSpawnedPlant();
                return;
            }
        }

        if (CurrentActivePlant == null) return;

        // 3. Evaluate Network Traversal
        HashSet<(int, int)> visitedCells = new HashSet<(int, int)>();

        if (EvaluateNetworkCell(startX, startY, visitedCells))
        {
            Debug.Log($"SUCCESS: Plant {CurrentActivePlant.plantName} grown successfully!");
            if (spawnedPlantInstance == null && CurrentActivePlant.plantPrefab != null)
            {
                Vector3 spawnPos = plantSpawnPoint != null ? plantSpawnPoint.position : GetEndPosition();
                spawnedPlantInstance = Instantiate(CurrentActivePlant.plantPrefab, spawnPos, Quaternion.identity);
            }
        }
        else
        {
            ClearSpawnedPlant();
        }
    }

    private bool EvaluateNetworkCell(int currX, int currY, HashSet<(int, int)> visited)
    {
        // 1. Check Win Condition at End Socket
        if (IsEndCell(currX, currY))
        {
            RunePiece endPiece = GetRunePiece(currX, currY);
            if (endPiece != null && endPiece.GetValueAtCell(currX, currY) == CurrentEndValue)
            {
                Debug.Log($"[Network] Reached End Socket at ({currX},{currY}) with matching value [{CurrentEndValue}]!");
                return true;
            }
            return false;
        }

        visited.Add((currX, currY));

        RunePiece currentPiece = GetRunePiece(currX, currY);
        if (currentPiece == null)
        {
            visited.Remove((currX, currY));
            return false;
        }

        int currentVal = currentPiece.GetValueAtCell(currX, currY);

        // 2. Check 4-directional neighbors
        (int dx, int dy)[] directions = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        foreach (var (dx, dy) in directions)
        {
            int nextX = currX + dx;
            int nextY = currY + dy;

            if (!IsValidLogicalCell(nextX, nextY) || visited.Contains((nextX, nextY)))
                continue;

            RunePiece neighborPiece = GetRunePiece(nextX, nextY);
            if (neighborPiece == null) continue;

            int neighborVal = neighborPiece.GetValueAtCell(nextX, nextY);

            // Valid step: internal piece transition OR matching adjacent value
            bool isSameDomino = (neighborPiece == currentPiece);
            bool isMatchingValue = (neighborVal == currentVal);

            if (isSameDomino || isMatchingValue)
            {
                if (EvaluateNetworkCell(nextX, nextY, visited))
                    return true;
            }
        }

        visited.Remove((currX, currY));
        return false;
    }

    private void ClearActivePlant()
    {
        CurrentStartValue = -1;
        CurrentEndValue = -1;
        CurrentActivePlant = null;
        ClearSpawnedPlant();
    }

    private void ClearSpawnedPlant()
    {
        if (spawnedPlantInstance != null)
        {
            Destroy(spawnedPlantInstance);
            spawnedPlantInstance = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (width <= 0 || height <= 0 || cellSize <= 0f) return;

        Gizmos.matrix = transform.localToWorldMatrix;

        // 1. Draw Grid Cells & Coordinates
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 localPos = GridOriginLocal + new Vector3(x * cellSize, y * cellSize, 0f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(localPos, new Vector3(cellSize, cellSize, 0.01f));

#if UNITY_EDITOR
                Vector3 worldPos = transform.TransformPoint(localPos);
                GUIStyle labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.yellow;
                labelStyle.fontSize = 10;
                labelStyle.alignment = TextAnchor.MiddleCenter;

                Handles.Label(worldPos, $"({x},{y})", labelStyle);
#endif
            }
        }

        // 2. Draw Start Socket (y = -1)
        if (showStartSocket)
        {
            Vector3 startLocal = GetStartPositionLocal();
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(startLocal, new Vector3(cellSize, cellSize, 0.01f));

#if UNITY_EDITOR
            Vector3 worldPos = transform.TransformPoint(startLocal);
            GUIStyle startStyle = new GUIStyle();
            startStyle.normal.textColor = Color.green;
            startStyle.fontSize = 11;
            startStyle.fontStyle = FontStyle.Bold;
            startStyle.alignment = TextAnchor.MiddleCenter;

            Handles.Label(worldPos, $"Start\n({width / 2},-1)", startStyle);
#endif
        }

        // 3. Draw End Socket (y = height)
        if (showEndSocket)
        {
            Vector3 endLocal = GetEndPositionLocal();
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(endLocal, new Vector3(cellSize, cellSize, 0.01f));

#if UNITY_EDITOR
            Vector3 worldPos = transform.TransformPoint(endLocal);
            GUIStyle endStyle = new GUIStyle();
            endStyle.normal.textColor = Color.cyan;
            endStyle.fontSize = 11;
            endStyle.fontStyle = FontStyle.Bold;
            endStyle.alignment = TextAnchor.MiddleCenter;

            Handles.Label(worldPos, $"End\n({width / 2},{height})", endStyle);
#endif
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
}