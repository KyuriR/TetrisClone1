using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public enum ControlSchemeII
{
    WASD,
    Arrows
}




public class PieceII : MonoBehaviour
{
    public Vector3Int Position => position;

    private BoardII board;
    private TetrominoDataII data;
    private Vector3Int position;

    public ControlSchemeII controlScheme;
    public void Initialize(BoardII board, Vector3Int spawnPosition, TetrominoDataII tetromino)
    {

        this.board = board;
        this.data = tetromino;
        this.position = spawnPosition;

        Draw();

        nextFallTime = Time.time + fallTime;
    }

  

    public TetrominoDataII Data => data;
    private void Draw()
    {
        foreach (Vector2Int cell in data.cells)
        {
            Vector3Int tilePosition = position + (Vector3Int)cell;
            board.activeTilemap.SetTile(tilePosition, data.tile);
        }
    }
    private void Update()
    {
        if (controlScheme == ControlSchemeII.WASD)
        {
            if (Input.GetKeyDown(KeyCode.A))
                Move(Vector3Int.left);

            if (Input.GetKeyDown(KeyCode.D))
                Move(Vector3Int.right);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Move(Vector3Int.left);

            if (Input.GetKeyDown(KeyCode.RightArrow))
                Move(Vector3Int.right);
        }

        if (Time.time >= nextFallTime)
        {
            Move(Vector3Int.down);
            nextFallTime = Time.time + fallTime;
        }
    }

    private void Move(Vector3Int direction)
    {
        Clear();

        Vector3Int newPosition = position + direction;

        if (board.IsValidPosition(this, newPosition))
        {
            position = newPosition;
            Draw();
        }
        else
        {
            // Put the piece back where it was
            Draw();

            // If we were trying to move down, the piece has landed
            if (direction == Vector3Int.down)
            {
                Lock();
            }
        }
    }

    private void Lock()
    {
        board.LockPiece(this);
    }

    private void Clear()
    {
        foreach (Vector2Int cell in data.cells)
        {
            Vector3Int tilePosition = position + (Vector3Int)cell;
            board.activeTilemap.SetTile(tilePosition, null);
        }
    }

    [SerializeField] private float fallTime = 1f;

    private float nextFallTime;
}