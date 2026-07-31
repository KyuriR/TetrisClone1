using UnityEngine;

public enum ControlScheme
{
    WASD,   // A/D move, S soft drop, W rotate
    Arrows  // Left/Right move, Down soft drop, Up rotate
}

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    // Set in the inspector — this is what makes one Piece instance
    // respond to WASD and the other to Arrow Keys, per the assignment's
    // core twist. Set once per Piece GameObject and never changed at runtime.
    public ControlScheme controlScheme = ControlScheme.WASD;

    // Remembered so Lock() can ask the Board to respawn THIS piece at its
    // own spawn point, not the other piece's — needed now that there are two.
    private Vector3Int spawnPosition;

    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float moveTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.data = data;
        this.board = board;
        this.position = position;
        this.spawnPosition = position;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        board.Clear(this);

        // Timer allowing the player to make adjustments before locking in place
        lockTime += Time.deltaTime;

        // Handle rotation — single rotate key per scheme (no separate CW/CCW,
        // since the assignment only specifies WASD vs Arrows, not extra keys).
        // Wall kicks (from Data.cs) still apply on every rotation attempt.
        if (RotateKeyPressed())
        {
            Rotate(1);
        }

        // Hard drop intentionally omitted for now — see project plan Section 1
        // scope notes: with only WASD/Arrows specified, there's no unclaimed
        // key to bind hard drop to for either piece without picking an
        // arbitrary extra key. Revisit as a team decision if you want it back.

        if (Time.time > moveTime)
        {
            HandleMoveInputs();
        }

        if (Time.time > stepTime)
        {
            Step();
        }

        board.Set(this);
    }

    // Returns which physical key to check for "rotate," based on this
    // instance's assigned control scheme.
    private bool RotateKeyPressed()
    {
        return controlScheme == ControlScheme.WASD
            ? Input.GetKeyDown(KeyCode.W)
            : Input.GetKeyDown(KeyCode.UpArrow);
    }

    private void HandleMoveInputs()
    {
        if (controlScheme == ControlScheme.WASD)
        {
            if (Input.GetKey(KeyCode.S))
            {
                if (Move(Vector2Int.down))
                {
                    stepTime = Time.time + stepDelay;
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                Move(Vector2Int.left);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Move(Vector2Int.right);
            }
        }
        else // ControlScheme.Arrows
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (Move(Vector2Int.down))
                {
                    stepTime = Time.time + stepDelay;
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Move(Vector2Int.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                Move(Vector2Int.right);
            }
        }
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        board.Set(this);
        board.ClearLines();

        // Respawn THIS piece specifically, at ITS OWN spawn point — this is
        // the key change from the single-piece tutorial, where SpawnPiece()
        // took no arguments because there was only ever one piece to respawn.
        board.SpawnPiece(this, spawnPosition);
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f;
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;

        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}