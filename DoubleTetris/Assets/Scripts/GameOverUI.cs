using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public Board board;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (board != null)
        {
            board.OnGameOver += HandleGameOver;
        }
    }

    private void OnDisable()
    {
        if (board != null)
        {
            board.OnGameOver -= HandleGameOver;
        }
    }

    private void HandleGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final score: " + board.score.ToString("N0");
        }
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ExitGame()
    {

        Application.Quit();

    }
}