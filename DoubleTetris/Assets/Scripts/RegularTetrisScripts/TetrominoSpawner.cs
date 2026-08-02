using Unity.VisualScripting;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    [Header("In-Game Spawn Settings")]
    [Tooltip("All 7 tetromino prefabes")]
    public GameObject[] tetrominoPrefabs;
    [Tooltip("World-space point where the next piece preview should appear ")]
    public Transform previewSpawnPoint;

    private int nextIndex;
    private GameObject currentPreview;

    void Start()
    {
        nextIndex = Random.Range(0, tetrominoPrefabs.Length);
    }

    public void SpawnNewTetromino()
    {
        Vector3 spawnPos = new Vector3(5,19,0);
        GameObject go = Instantiate(tetrominoPrefabs[nextIndex], spawnPos, Quaternion.identity);

        var tet = go.GetComponent<Tetromino>();
        //tet.fallTime = GameManager.Instance.CurrentFallTime;

        nextIndex = Random.Range(0, tetrominoPrefabs.Length);
        UpdatePreview();

    }

    private void UpdatePreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);

            currentPreview = Instantiate((tetrominoPrefabs[nextIndex]) , previewSpawnPoint.position, Quaternion.identity);

            var tScript = currentPreview.GetComponent<Tetromino>();
           /* if(tScript != null)
            {
                tScript.enabled = false;
            }*/
        }
    }

    public void ClearPreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
    }
}
