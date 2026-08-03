using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    // Two active pieces instead of one. Assign these in the inspector to the
    // two Piece components under this Board � set each one's Control Scheme
    // field (WASD / Arrows) to match.
    public Piece activePieceA;
    public Piece activePieceB;

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    // Two spawn points, kept apart so the pieces don't spawn overlapping.
    // Board is 10 wide (x range -5..4) � left/right lanes with a gap between.
    public Vector3Int spawnPositionA = new Vector3Int(-3, 8, 0);
    public Vector3Int spawnPositionB = new Vector3Int(2, 8, 0);

    // Team decision (project plan Section 2/3.9): does the game end if EITHER
    // piece fails to spawn, or only if BOTH do? Exposed as a toggle rather
    // than hardcoded, since this is a real design decision, not just a bug fix.
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

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece(activePieceA, spawnPositionA);
        SpawnPiece(activePieceB, spawnPositionB);
    }

    private void OnDrawGizmos()
    {
        // Draw the playable board boundary
        Gizmos.color = Color.cyan;
        RectInt bounds = Bounds;
        Vector3 center = new Vector3(
            bounds.x + bounds.width / 2f,
            bounds.y + bounds.height / 2f,
            0f
        );
        Vector3 size = new Vector3(bounds.width, bounds.height, 1f);
        Gizmos.DrawWireCube(center, size);

        // Draw individual cell grid lines
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

        // Mark the two spawn points so you can eyeball overlap issues
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnPositionA, 0.4f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(spawnPositionB, 0.4f);
    }

    // Takes which piece and where, instead of assuming a single activePiece.
    // Each Piece remembers its own spawn position (set inside Initialize) so
    // it can request a respawn at the right spot after locking.
    public void SpawnPiece(Piece piece, Vector3Int spawnPosition)
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

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
            // else: unresolved team decision � what happens to just one side
            // if only its spawn is blocked while the other still has room.
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
    // move and re-sets them after (which Piece.cs's Update loop already does),
    // tilemap.HasTile reflects the OTHER active piece's current cells here,
    // exactly the same way it reflects the locked stack. This is what resolves
    // the original "how do you control both without restricting the other"
    // problem from the project plan's Phase 2 design evolution.
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

    // Team decision (assumed): ONE shared score, matching how real Tetris only
    // ever has a single score � not two separate per-player scores. If the
    // team decides separate scores instead, this needs restructuring.
    public int score { get; private set; }
    public int totalLinesCleared { get; private set; }

    // Level increases every 10 total lines cleared � same pacing as the
    // original game � and scales the score awarded per clear below.
    public int Level => totalLinesCleared / 10;

    // Classic original-Tetris line-clear scores, indexed by how many lines
    // clear AT ONCE in a single lock (not lifetime total): 1/2/3/4 lines.
    // Index 0 is unused padding so the array lines up with lineCount directly.
    private static readonly int[] LineClearScores = { 0, 40, 100, 300, 1200 };

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int linesClearedThisLock = 0;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row))
            {
                LineClear(row);
                linesClearedThisLock++;
            }
            else
            {
                row++;
            }
        }

        if (linesClearedThisLock > 0)
        {
            AwardScore(linesClearedThisLock);
        }
    }

    // Awards points for however many lines cleared in one lock event, scaled
    // by the current level, and advances totalLinesCleared/Level for next time.
    private void AwardScore(int linesClearedThisLock)
    {
        int index = Mathf.Clamp(linesClearedThisLock, 0, LineClearScores.Length - 1);
        int awarded = LineClearScores[index] * (Level + 1);

        score += awarded;
        totalLinesCleared += linesClearedThisLock;

        // TEMPORARY � replace with a real UI/HUD call once that system exists.
        Debug.Log($"Cleared {linesClearedThisLock} line(s) � +{awarded} points (Level {Level}). Total score: {score}");
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

    // ---- Wrapper methods matching the team's shared contract (project plan
    // Section 0's Task 0.1) � Collision/Input/PieceController owners on other
    // systems should call these three, not the tilemap directly, so they stay
    // decoupled from this class's internal Tilemap implementation. ----

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