using UnityEngine;
using UnityEngine.Tilemaps;

public enum TetrominoS
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z
}

[CreateAssetMenu(fileName = "TetrominoII", menuName = "Tetromino")]
public class TetrominoDataII : ScriptableObject
{
    public TetrominoS tetromino;
    public Tile tile;
    public Vector2Int[] cells;
}