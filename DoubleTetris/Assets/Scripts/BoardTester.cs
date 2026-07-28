using UnityEngine;

// Attach this to any empty GameObject in the test scene, assign the Board
// reference in the inspector, press Play, and read the Console.
// This does NOT need real Piece Controller/Input to exist — it manually
// pokes tiles into the board and checks the Grid Manager's own methods.
public class BoardTester : MonoBehaviour
{
    public Board board;

    private void Start()
    {
        Debug.Log("---- BoardTester: starting hardcoded grid tests ----");

        TestIsCellOccupied();
        TestIsRowFull();
        TestLockPieceAndLineClear();

        Debug.Log("---- BoardTester: all tests finished ----");
    }

    private void TestIsCellOccupied()
    {
        // Manually place a single fake tile using the board's own Tilemap,
        // bypassing Piece entirely, then check IsCellOccupied agrees.
        Vector3Int testCell = new Vector3Int(0, 0, 0);
        board.tilemap.SetTile(testCell, board.tetrominoes[0].tile);

        bool occupied = board.IsCellOccupied(0, 0);
        bool empty = board.IsCellOccupied(0, 1); // untouched cell, should be false

        Debug.Log(occupied
            ? "PASS: IsCellOccupied correctly reports a filled cell as occupied"
            : "FAIL: IsCellOccupied did not detect a manually filled cell");

        Debug.Log(!empty
            ? "PASS: IsCellOccupied correctly reports an empty cell as unoccupied"
            : "FAIL: IsCellOccupied reported an empty cell as occupied");

        // Clean up so this test doesn't interfere with the next one
        board.tilemap.SetTile(testCell, null);
    }

    private void TestIsRowFull()
    {
        RectInt bounds = board.Bounds;
        int testRow = bounds.yMin; // bottom row

        // Fill every column in the test row with fake tiles
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            board.tilemap.SetTile(new Vector3Int(col, testRow, 0), board.tetrominoes[0].tile);
        }

        bool fullDetected = board.IsRowFull(testRow);
        Debug.Log(fullDetected
            ? "PASS: IsRowFull correctly detects a completely filled row"
            : "FAIL: IsRowFull did not detect a fully filled row");

        // Leave one cell empty and confirm it's no longer reported as full
        board.tilemap.SetTile(new Vector3Int(bounds.xMin, testRow, 0), null);
        bool partialDetected = board.IsRowFull(testRow);
        Debug.Log(!partialDetected
            ? "PASS: IsRowFull correctly reports a partially filled row as NOT full"
            : "FAIL: IsRowFull incorrectly reported a partial row as full");

        // Clean up
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            board.tilemap.SetTile(new Vector3Int(col, testRow, 0), null);
        }
    }

    private void TestLockPieceAndLineClear()
    {
        RectInt bounds = board.Bounds;
        int testRow = bounds.yMin + 1; // pick a row away from any spawn interference

        // Pre-fill every column EXCEPT one, using raw tiles (simulating an
        // already-locked stack from earlier play).
        for (int col = bounds.xMin; col < bounds.xMax - 1; col++)
        {
            board.tilemap.SetTile(new Vector3Int(col, testRow, 0), board.tetrominoes[0].tile);
        }

        // Build a hardcoded fake single-cell "piece" occupying the one
        // remaining gap, using the stub Piece component so LockPiece's
        // full path (Set + ClearLines) gets exercised, not just ClearLines.
        GameObject fakeGO = new GameObject("FakeTestPiece");
        Piece fakePiece = fakeGO.AddComponent<Piece>();

        TetrominoData fakeData = board.tetrominoes[0];
        Vector3Int fakePosition = new Vector3Int(bounds.xMax - 1, testRow, 0);

        // Override cells to a single offset (0,0) so this "piece" is exactly
        // one cell — the one gap left in the row above.
        fakePiece.Initialize(board, fakePosition, fakeData);
        fakePiece.SetCellsForTesting(new Vector3Int[] { Vector3Int.zero });

        bool rowFullBeforeLock = board.IsRowFull(testRow);
        Debug.Log(!rowFullBeforeLock
            ? "PASS: row correctly NOT full before locking the final piece"
            : "FAIL: row was already reported full before the gap was filled");

        board.LockPiece(fakePiece);

        bool rowClearedAfterLock = !board.tilemap.HasTile(new Vector3Int(bounds.xMin, testRow, 0));
        Debug.Log(rowClearedAfterLock
            ? "PASS: LockPiece filled the gap, detected the full row, and cleared it"
            : "FAIL: row was not cleared after LockPiece — check Set/ClearLines wiring");

        Destroy(fakeGO);
    }
}
