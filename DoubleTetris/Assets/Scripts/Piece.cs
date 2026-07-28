using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        if (cells == null) {
            cells = new Vector3Int[data.cells.Length];
        }

        // data.cells is Vector2Int (2D shape offsets) — convert to Vector3Int
        // so it can be added directly to the Vector3Int tilemap position.
        for (int i = 0; i < cells.Length; i++) {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    // Exposed so BoardTester (or later, the real Piece Controller) can force
    // a piece into a specific position without needing real input/gravity yet.
    public void SetPosition(Vector3Int newPosition)
    {
        board.Clear(this);
        position = newPosition;
        board.Set(this);
    }

    // TEST-ONLY: lets BoardTester construct a fake piece with a custom shape
    // (e.g. a single cell) without needing a real TetrominoData entry for it.
    // Not meant to be called by real gameplay code.
    public void SetCellsForTesting(Vector3Int[] testCells)
    {
        cells = testCells;
    }
}
