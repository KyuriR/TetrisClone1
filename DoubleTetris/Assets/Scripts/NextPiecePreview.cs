using UnityEngine;
using UnityEngine.Tilemaps;

public class NextPiecePreview : MonoBehaviour
{
    public Tilemap tilemap;

    public void Show(TetrominoData data)
    {
        Debug.Log("Preview Updated!");

        Clear();

        foreach (Vector2Int cell in data.cells)
        {
            Vector3Int pos = new Vector3Int(cell.x, cell.y, 0);

            Debug.Log("Drawing tile at " + pos);

            tilemap.SetTile(pos, data.tile);
        }
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
}