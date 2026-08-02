using UnityEngine;
using UnityEngine.Tilemaps;

public class PieceII : MonoBehaviour
{
    private BoardII board;
    private TetrominoDataII data;
    private Vector3Int position;

    public void Initialize(BoardII board, Vector3Int spawnPosition, TetrominoDataII tetromino)
    {
        this.board = board;
        this.data = tetromino;
        this.position = spawnPosition;

        Draw();
    }

    private void Draw()
    {
        foreach (Vector2Int cell in data.cells)
        {
            Vector3Int tilePosition = position + (Vector3Int)cell;
            board.tilemap.SetTile(tilePosition, data.tile);
        }
    }
}