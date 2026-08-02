using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardII : MonoBehaviour
{
    public Tilemap tilemap;


    [Header("Tetrominoes")]
    public TetrominoDataII[] tetrominoes;

    [Header("Pieces")]
    public PieceII leftPiece;
    public PieceII rightPiece;

    public Vector3Int leftSpawn = new Vector3Int(-3, 9, 0);
    public Vector3Int rightSpawn = new Vector3Int(3, 9, 0);

    public int boardWidth = 10;
    public int boardHeight = 20;

    private void Awake()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();
    }

    private void Start()
    {
        SpawnPiece(leftPiece, leftSpawn);
        SpawnPiece(rightPiece, rightSpawn);
    }

    public void SpawnPiece(PieceII piece, Vector3Int spawnPosition)
    {
        TetrominoDataII data = tetrominoes[Random.Range(0, tetrominoes.Length)];
        piece.Initialize(this, spawnPosition, data);
    }

    public bool IsValidPosition(PieceII piece, Vector3Int position)
    {
        foreach (Vector2Int cell in piece.Data.cells)
        {
            Vector3Int tilePosition = position + (Vector3Int)cell;

            // Left wall
            if (tilePosition.x < -boardWidth / 2)
                return false;

            // Right wall
            if (tilePosition.x >= boardWidth / 2)
                return false;

            // Bottom
            if (tilePosition.y < 0)
                return false;
        }

        return true;
    }
}