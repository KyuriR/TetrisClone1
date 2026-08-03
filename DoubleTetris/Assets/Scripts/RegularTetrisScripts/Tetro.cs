/*using UnityEngine;

public class Tetro : MonoBehaviour
{
    public enum PieceType { I, J, L, O, S, T, Z}

    [Header("Place Settings")]

    [SerializeField] private PieceType pieceType;
    [SerializeField] private bool canRotate = true;

    [Header("TimingSettings")]

    public float fallTime;

    [SerializeField] private float moveRepeatDelay = 0.15f;
    [SerializeField] private float moveRepeatRate = 0.05f;
    [SerializeField] private float softDropRepeatRate = 0.05f;

    private TetrisControls controls;
    private float heldMoveDirection;
    private bool moveHeld;
    private bool rotateRequested;
    private float fallTimer, moveTimer, softDropTimer;

    private int rotationIndex = 0;

    private static readonly Vector2Int[][] JLSTZ_CW_Kicks = new[]
    {
        //0-1: Push right
        new[]{ new Vector2Int(0,0),
        new Vector2Int(1,0),
        new Vector2Int(1,1),
        new Vector2Int(0,-2),
        new Vector2Int(1,-2),
        new Vector2Int(2,0)},

        //1-2: Push LEFT
        new[]{ new Vector2Int(0,0),
        new Vector2Int(-1,0),
        new Vector2Int(-1,-1),
        new Vector2Int(0,2),
        new Vector2Int(-1,2),
        new Vector2Int(-2,0)},

        //2-3: Push LEFT
        new[]{ new Vector2Int(0,0),
        new Vector2Int(-1,0),
        new Vector2Int(-1,1),
        new Vector2Int(0,-2),
        new Vector2Int(-1,-2),
        new Vector2Int(-2,0)},

        //3-0: Push right
        new[]{ new Vector2Int(0,0),
        new Vector2Int(1,0),
        new Vector2Int(1,-1),
        new Vector2Int(0,2),
        new Vector2Int(1,2),
        new Vector2Int(2,0)},
        
    }
    
}
*/