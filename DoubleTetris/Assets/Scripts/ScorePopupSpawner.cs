using UnityEngine;

public class ScorePopupSpawner : MonoBehaviour
{
    public Board board;
    public GameObject popupPrefab;

    private void OnEnable()
    {
        if (board != null)
        {
            board.OnScoreAwarded += HandleScoreAwarded;
        }
    }

    private void OnDisable()
    {
        if (board != null)
        {
            board.OnScoreAwarded -= HandleScoreAwarded;
        }
    }

    private void HandleScoreAwarded(int linesCleared, int points, Vector3 position)
    {
        GameObject popup = Instantiate(popupPrefab, position, Quaternion.identity);
        ScorePopup popupScript = popup.GetComponent<ScorePopup>();
        if (popupScript != null)
        {
            popupScript.Initialize(points);
        }
    }
}
