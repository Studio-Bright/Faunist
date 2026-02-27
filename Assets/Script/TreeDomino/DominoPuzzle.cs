using UnityEngine;
using System.Collections.Generic;

public class DominoPuzzle : MonoBehaviour
{
    [Header("Domino Setup")]
    public GameObject dominoPrefab;   
    public Transform spawnArea;
    public Transform boardArea;
    public int piecesToGenerate = 9;
    public int maxPipValue = 6;      
    public bool isInteractable = false;

    private List<DominoPiece> currentPieces = new List<DominoPiece>();

    public GameObject pipPrefab;

    [Header("Grid Settings")]
    public float cellSize = 0.4f;   // One square size (matches domino width)
    public Vector3 gridOrigin;      // Bottom-left of board
    void Awake()
    {
        CalculateGridOrigin();
    }

    void CalculateGridOrigin()
    {
        if (boardArea == null) return;

        Collider col = boardArea.GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("Board needs a Collider!");
            return;
        }

        Bounds bounds = col.bounds;

        // Bottom-left corner of the board
        gridOrigin = new Vector3(
            bounds.min.x,
            bounds.min.y,
            bounds.min.z
        );
    }


    public void GeneratePieces()
    {
        foreach (var piece in currentPieces)
            if (piece != null) Destroy(piece.gameObject);
        currentPieces.Clear();

        for (int i = 0; i < piecesToGenerate; i++)
        {
            Vector3 randomPos = spawnArea.position + new Vector3(Random.Range(-1f, 1f), 0.1f, Random.Range(-1f, 1f));
            GameObject pieceObj = Instantiate(dominoPrefab, randomPos, Quaternion.identity);
            DominoPiece piece = pieceObj.GetComponent<DominoPiece>();

            piece.valueA = Random.Range(1, maxPipValue);
            piece.valueB = Random.Range(1, maxPipValue);

            piece.pipPrefab = pipPrefab;

            piece.GeneratePips();

            piece.parentPuzzle = this;

            currentPieces.Add(piece);
        }

        Debug.Log("Domino Puzzle: Pieces generated!");
    }

    public void CheckPuzzle()
    {
        if (!isInteractable) return;

        foreach (var piece in currentPieces)
        {
            Debug.Log("Checking piece: " + piece.name);

            if (!piece.IsConnectedCorrectly())
            {
                Debug.Log("Piece failed: " + piece.name);
                return;
            }
        }

        Debug.Log("Domino Puzzle Completed!");
        GeneratePieces();
    }


}
