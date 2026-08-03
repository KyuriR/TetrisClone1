using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public NextPiecePreview previewA;
    public NextPiecePreview previewB;

    private TetrominoData nextPieceA;
    private TetrominoData nextPieceB;

    public Piece activePieceA;
    public Piece activePieceB;

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public Vector3Int spawnPositionA = new Vector3Int(-3, 8, 0);
    public Vector3Int spawnPositionB = new Vector3Int(2, 8, 0);

    public bool endGameOnEitherBlocked = true;

    public bool isGameOver { get; private set; }

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

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        nextPieceA = tetrominoes[Random.Range(0, tetrominoes.Length)];
        nextPieceB = tetrominoes[Random.Range(0, tetrominoes.Length)];

        previewA.Show(nextPieceA);
        previewB.Show(nextPieceB);

        SpawnPiece(activePieceA, spawnPositionA);
        SpawnPiece(activePieceB, spawnPositionB);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        RectInt bounds = Bounds;
        Vector3 center = new Vector3(
            bounds.x + bounds.width / 2f,
            bounds.y + bounds.height / 2f,
            0f
        );
        Vector3 size = new Vector3(bounds.width, bounds.height, 1f);
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = new Color(1f, 1f, 1f, 0.15f);
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            Gizmos.DrawLine(
                new Vector3(x, bounds.yMin, 0f),
                new Vector3(x, bounds.yMax, 0f)
            );
        }
        for (int y = bounds.yMin; y <= bounds.yMax; y++)
        {
            Gizmos.DrawLine(
                new Vector3(bounds.xMin, y, 0f),
                new Vector3(bounds.xMax, y, 0f)
            );
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnPositionA, 0.4f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(spawnPositionB, 0.4f);
    }

    public void SpawnPiece(Piece piece, Vector3Int spawnPosition)
    {
        if (isGameOver)
        {
            return;
        }

        TetrominoData data;

        if (piece == activePieceA)
        {
            data = nextPieceA;
            nextPieceA = tetrominoes[Random.Range(0, tetrominoes.Length)];
            previewA.Show(nextPieceA);
        }
        else
        {
            data = nextPieceB;
            nextPieceB = tetrominoes[Random.Range(0, tetrominoes.Length)];
            previewB.Show(nextPieceB);
        }

        piece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(piece, spawnPosition))
        {
            Set(piece);
        }
        else
        {
            if (endGameOnEitherBlocked)
            {
                GameOver();
            }
        }
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        if (activePieceA != null)
        {
            activePieceA.enabled = false;
        }

        if (activePieceB != null)
        {
            activePieceB.enabled = false;
        }

        Debug.Log($"GAME OVER. Final score: {score}");
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

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public int score { get; private set; }
    public int totalLinesCleared { get; private set; }
    public int Level => totalLinesCleared / 10;
    public event System.Action<int> OnRowCleared;
    public event System.Action<int, int, Vector3> OnScoreAwarded;

    private static readonly int[] LineClearScores = { 0, 40, 100, 300, 1200 };

    public void ClearLines(Piece lockedPiece = null)
    {
        Piece other = null;
        if (lockedPiece == activePieceA) other = activePieceB;
        else if (lockedPiece == activePieceB) other = activePieceA;

        if (other != null)
        {
            Clear(other);
        }

        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int linesClearedThisLock = 0;
        System.Collections.Generic.List<int> clearedRows = new System.Collections.Generic.List<int>();

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                OnRowCleared?.Invoke(row);
                LineClear(row);
                linesClearedThisLock++;
                clearedRows.Add(row);
            }
            else
            {
                row++;
            }
        }

        if (linesClearedThisLock > 0)
        {
            AwardScore(linesClearedThisLock, clearedRows);
        }

        if (other != null)
        {
            Set(other);
        }
    }

    private void AwardScore(int linesClearedThisLock, System.Collections.Generic.List<int> clearedRows)
    {
        int index = Mathf.Clamp(linesClearedThisLock, 0, LineClearScores.Length - 1);
        int awarded = LineClearScores[index] * (Level + 1);

        score += awarded;
        totalLinesCleared += linesClearedThisLock;

        float averageRow = 0f;
        foreach (int r in clearedRows)
        {
            averageRow += r;
        }
        averageRow /= clearedRows.Count;

        RectInt bounds = Bounds;
        Vector3 popupPosition = new Vector3(bounds.xMin + bounds.width / 2f, averageRow + 0.5f, 0f);

        OnScoreAwarded?.Invoke(linesClearedThisLock, awarded, popupPosition);

        Debug.Log($"Cleared {linesClearedThisLock} line(s), plus {awarded} points, Level {Level}. Total score: {score}");
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
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
        ClearLines(piece);
    }
}