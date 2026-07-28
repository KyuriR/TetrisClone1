using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePieceA;
    public Piece activePieceB;

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public Vector3Int spawnPositionA = new Vector3Int(-3, 8, 0);
    public Vector3Int spawnPositionB = new Vector3Int(2, 8, 0);

    public bool endGameOnEitherBlocked = true;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece(activePieceA, spawnPositionA);
        SpawnPiece(activePieceB, spawnPositionB);
    }

    public void SpawnPiece(Piece piece, Vector3Int spawnPosition)
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        piece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(piece, spawnPosition)) {
            Set(piece);
        } else {
            if (endGameOnEitherBlocked) {
                GameOver();
            }
            // else: unresolved — team decision on what happens to just one
            // side if only its spawn is blocked while the other still has room.
        }
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();
        // Do anything else you want on game over here..
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    // Piece-vs-wall, piece-vs-stack, AND piece-vs-other-piece collision all in
    // one check: as long as each Piece clears its own cells before testing a
    // move and re-sets them after, tilemap.HasTile already reflects the OTHER
    // active piece's current cells here, same as it reflects the locked stack.
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row)) {
                LineClear(row);
            } else {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }


    public bool IsCellOccupied(int x, int y)
    {
        return tilemap.HasTile(new Vector3Int(x, y, 0));
    }

    public bool IsRowFull(int y)
    {
        return IsLineFull(y);
    }

    public void LockPiece(Piece piece)
    {
        Set(piece);
        ClearLines();
    }
}
