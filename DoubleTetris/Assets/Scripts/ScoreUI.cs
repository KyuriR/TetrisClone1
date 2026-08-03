using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public Board board;
    public TMP_Text scoreText;

    private void Update()
    {
        if (board != null && scoreText != null)
        {
            scoreText.text = "Score: " + board.score.ToString("N0");
        }
    }
}
