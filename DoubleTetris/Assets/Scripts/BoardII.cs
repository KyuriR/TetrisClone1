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
}