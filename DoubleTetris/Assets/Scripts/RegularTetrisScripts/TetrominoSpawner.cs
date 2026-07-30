using System;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    [Header("In-Game Spawn Settings")]
    [Tooltip("All 7 tetro prefabes")]
    public GameObject[] tetrominoPrefabs;
    [Tooltip("would-space point where the next pies ")]
    public Transform previewSpawnPoint;

    private int nextIndex;
    private GameObject currentPreview;

    void Start()
    {
        nextIndex = Random.range(0, tetrominoPrefabs.Length);
    }

    public void SpawnNewTetromino()
    {
        Vector3 spawnPos = new Vector3(5,19,0);
        GameObject go = Instantiate(tetrominoPrefabs[nextIndex], spawnPos, Quaternion.identity);

        var tet = go.GetComponent<Tetromino>();
        tet.fallTime = GameManager.Instance.currentFallTime;

        

    }
}
