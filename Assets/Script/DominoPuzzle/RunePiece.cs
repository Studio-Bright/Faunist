using UnityEngine;

public class RunePiece : PickupItem
{
    [HideInInspector] public DominoBoard dominoBoard;

    [Header("Domino Properties")]
    public bool horizontal = true;

    [Header("Rune Values")]
    public int valueA = 1;
    public int valueB = 1;

    private int rotationStep = 0;
    private bool placed = false;
    private int placedX = -1;
    private int placedY = -1;

    public bool IsPlaced => placed;

    private void Start()
    {
        dominoBoard = FindFirstObjectByType<DominoBoard>();
        if (dominoBoard == null) Debug.LogError("RunePiece could not find DominoBoard!");
    }

    public override void OnPickup()
    {
        if (placed && dominoBoard != null)
        {
            dominoBoard.RemoveDomino(placedX, placedY, horizontal);
        }

        placed = false;
        placedX = -1;
        placedY = -1;

        base.OnPickup();
    }

    public override void OnDrop(Vector3 position)
    {
        placed = false;
        placedX = -1;
        placedY = -1;

        base.OnDrop(position);
    }

    public bool TryPlace(Vector3 boardPosition)
    {
        if (dominoBoard == null && (dominoBoard = FindFirstObjectByType<DominoBoard>()) == null) return false;
        if (placed) return false;

        if (!dominoBoard.WorldToCell(boardPosition, out int x, out int y)) return false;

        // Force Start/End vertical socket alignment
        if (dominoBoard.IsStartCell(x, y) || dominoBoard.IsStartCell(x, y - 1))
        {
            if (horizontal) return false;
            x = dominoBoard.width / 2;
            y = -1;
        }
        else if (dominoBoard.IsEndCell(x, y) || dominoBoard.IsEndCell(x, y + 1))
        {
            if (horizontal) return false;
            x = dominoBoard.width / 2;
            y = dominoBoard.height - 1;
        }

        if (!dominoBoard.CanPlaceDomino(x, y, horizontal)) return false;

        placedX = x;
        placedY = y;
        placed = true;

        transform.SetParent(null);
        transform.position = dominoBoard.GetDominoPosition(x, y, horizontal);
        ApplyBoardRotation();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        SetVisible(true);
        SetColliders(true);

        dominoBoard.PlaceDomino(x, y, horizontal, this);

        ValidateAdjacentConnections();
        return true;
    }

    public void RotateDomino()
    {
        if (!placed || dominoBoard == null) return;

        int oldX = placedX;
        int oldY = placedY;
        bool oldHorizontal = horizontal;

        dominoBoard.RemoveDomino(oldX, oldY, oldHorizontal);

        int nextRotation = (rotationStep + 1) % 4;
        bool nextHorizontal = nextRotation == 0 || nextRotation == 2;

        if (dominoBoard.IsStartCell(oldX, oldY) || dominoBoard.IsEndCell(oldX, oldY + 1))
        {
            nextHorizontal = false;
        }

        if (!dominoBoard.CanPlaceDomino(oldX, oldY, nextHorizontal))
        {
            dominoBoard.PlaceDomino(oldX, oldY, oldHorizontal, this);
            return;
        }

        rotationStep = nextRotation;
        horizontal = nextHorizontal;

        transform.position = dominoBoard.GetDominoPosition(placedX, placedY, horizontal);
        ApplyBoardRotation();

        dominoBoard.PlaceDomino(placedX, placedY, horizontal, this);
        ValidateAdjacentConnections();
    }

    private void ApplyBoardRotation()
    {
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        transform.Rotate(Vector3.forward, rotationStep * 90f, Space.World);
    }

    public (int x, int y) GetOtherHalfCell(int x, int y)
    {
        int secondX = horizontal ? placedX + 1 : placedX;
        int secondY = horizontal ? placedY : placedY + 1;

        if (x == placedX && y == placedY) return (secondX, secondY);
        return (placedX, placedY);
    }

    public int GetValueAtCell(int x, int y)
    {
        int secondX = horizontal ? placedX + 1 : placedX;
        int secondY = horizontal ? placedY : placedY + 1;

        bool isPrimaryCell = (x == placedX && y == placedY);
        bool isSecondaryCell = (x == secondX && y == secondY);

        if (!isPrimaryCell && !isSecondaryCell) return -1;

        // rotationStep 0: A top/left, B bottom/right
        // rotationStep 1: A right, B left
        // rotationStep 2: B top/left, A bottom/right
        // rotationStep 3: B right, A left
        if (rotationStep == 0 || rotationStep == 1)
        {
            return isPrimaryCell ? valueA : valueB;
        }
        else
        {
            return isPrimaryCell ? valueB : valueA;
        }
    }

    private void ValidateAdjacentConnections()
    {
        int cellAX = placedX;
        int cellAY = placedY;
        int cellBX = horizontal ? placedX + 1 : placedX;
        int cellBY = horizontal ? placedY : placedY + 1;

        CheckNeighborMatch(cellAX, cellAY);
        CheckNeighborMatch(cellBX, cellBY);
    }

    private void CheckNeighborMatch(int myX, int myY)
    {
        (int dx, int dy)[] directions = { (-1, 0), (1, 0), (0, -1), (0, 1) };
        int myValue = GetValueAtCell(myX, myY);

        foreach (var (dx, dy) in directions)
        {
            int otherX = myX + dx;
            int otherY = myY + dy;

            if (!dominoBoard.IsValidLogicalCell(otherX, otherY)) continue;

            RunePiece neighbor = dominoBoard.GetRunePiece(otherX, otherY);
            if (neighbor == null || neighbor == this) continue;

            int neighborValue = neighbor.GetValueAtCell(otherX, otherY);
            if (myValue != neighborValue)
            {
                Debug.LogWarning($"MISMATCH: [{name}] value {myValue} touches [{neighbor.name}] value {neighborValue} at ({otherX},{otherY})");
            }
        }
    }
}