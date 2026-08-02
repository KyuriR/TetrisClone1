using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I, J, L, O, S, T, Z
}

[System.Serializable]
public struct TetrominoData
{
    public Tile tile;
    public Tetromino tetromino;
    public Vector2Int[] cells { get; private set; }
    public Vector2Int[,] wallKicks { get; private set; }

    // Pulls this piece's shape and wall-kick data out of Data.cs by its
    // Tetromino type. Keeping this as a lookup (rather than hardcoding shape
    // data per-instance) means the Board's inspector array only ever needs a
    // Tetromino dropdown + a Tile — the actual shape logic lives in one place.
    public void Initialize()
    {
        cells = Data.Cells[tetromino];
        wallKicks = Data.WallKicks[tetromino];
    }
}