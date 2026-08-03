using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set; }

    [Header("UI References")]

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI linesText;
    public TextMeshProUGUI levelText;

    public GameObject gameOverText;
    public TextMeshProUGUI highScoresText;

    public Button pauseButton;
    public Button startNewGameButton;
    public TextMeshProUGUI pauseButtonText;

    [Header("Spawner")]
    public TetrominoSpawner spawner;
    
    [Header("Scoring Settingds")]

    public int basePlacementPoints = 50;

    [Header("Speed Settings")]
    public float baseFallTime = 1f;
    public float speedUpFactor = 0.9f;

    private int score;
    private int linesCleared;
    private int level;
    private bool isGameRunning;
    private bool isPaused;

    private const int k_MaxHighScore = 5;


    private List<int> highScore = new List<int>(k_MaxHighScore);

    public bool IsGameRunning => isGameRunning;
    public bool IsPaused => isPaused;

    public float CurrentFallTime =>
        Mathf.Max(0.05f, baseFallTime * Mathf.Pow(speedUpFactor,level - 1));

    
    [Header("Sound settings")]
    [SerializeField] AudioClip negativeBeep;
    [SerializeField] AudioClip rotateBlock;
    [SerializeField] AudioClip placeBlock;
    [SerializeField] AudioClip lineClear;
    [SerializeField] AudioClip levelUp;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] AudioClip activeGameMusic;
    [SerializeField] AudioClip inactiveGameMusic;
    [SerializeField] AudioSource musicPlayer;

    private void Awake()
    {
        if(Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        pauseButton.onClick.AddListener(TogglePause);
        startNewGameButton.onClick.AddListener(StartNewGame);

        LoadHighScores();
        DisplayHighScores();

        isGameRunning = false;
        isPaused = false;
        Time.timeScale = 0f;
        pauseButtonText.text = "PAUSE";
        gameOverText.gameObject.SetActive(false);
        UpdateUI();

        EventSystem.current.SetSelectedGameObject(startNewGameButton.gameObject);

    }

    #region Game Flow

    public void TogglePause()
    {
        if (!isGameRunning) return;
        if (isPaused) ResumeGame(); else PauseGame();
    }

    public void StartNewGame()
    {
        gameOverText.gameObject.SetActive(false);
        ClearGrid();
        score = linesCleared = 0;
        level = 1;
        UpdateUI();

        spawner.ClearPreview();
        spawner.SpawnNewTetromino();

        isGameRunning = true;
        isPaused = false;
        Time.timeScale = 1f;
        pauseButtonText.text = "PAUSE";

        EventSystem.current.SetSelectedGameObject(null);

        musicPlayer.Stop();
        musicPlayer.clip = activeGameMusic;
        musicPlayer.Play();
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseButtonText.text = "RESUME";
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseButtonText.text = "PAUSE";
    }

    private void GameOver()
    {
        isGameRunning = false;
        isPaused = false;
        Time.timeScale = 0f;
        gameOverText.gameObject.SetActive(true);

        AddToHighScores(score);
        DisplayHighScores();

        EventSystem.current.SetSelectedGameObject(startNewGameButton.gameObject);

        musicPlayer.Stop();
        musicPlayer.clip = inactiveGameMusic;
        musicPlayer.Play();
        
    }

    #endregion

    #region Scoring & level
    
    public void AddClearedLines(int count)
    {
        if(count <= 0) return;
        linesCleared += count;
        score += GetPointsFor(count)*level;
        int previousLevel = level;
        level = (linesCleared / 10) + 1;
        if(level > previousLevel)
        {
            PlayLevelUp();
        }
        UpdateUI();
    }

    public void AddPlacementScore()
    {
        score += basePlacementPoints * level;
        UpdateUI();
    }

    private int GetPointsFor(int rows)
    {
        switch (rows)
        {
            case 1: return 100;
            case 2: return 300;
            case 3: return 500;
            case 4: return 800;
            default: return 0;
        }
    }
    
    private void UpdateUI()
    {
        scoreText.text = score.ToString();
        linesText.text = linesCleared.ToString();
        levelText.text = level.ToString();
    }

    #endregion

    #region High Scores

    private void LoadHighScores()
    {
        highScore.Clear();
        for (int i = 1; i <= k_MaxHighScore; i++)
            highScore.Add(PlayerPrefs.GetInt("$HighScore(i))", 0));
    }

    private void SaveHighScore()
    {
        for (int i = 0; i <= k_MaxHighScore; i++)
            PlayerPrefs.GetInt("$HighScore(i+1))", highScore[i]);
        PlayerPrefs.Save();
    }

    private void AddToHighScores(int newScore)
    {
        for (int i = 0; i < highScore.Count; i++)
        {
            if (newScore > highScore[i])
            {
                highScore.Insert(i, newScore);
                if (highScore.Count > k_MaxHighScore)
                    highScore.RemoveAt(highScore.Count - 1);
                SaveHighScore();
                return;
            }
        }

        if (highScore.Count < k_MaxHighScore)
        {
            highScore.Add(newScore);
            SaveHighScore();
        }

    }

    private void DisplayHighScores()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < k_MaxHighScore; i++)
        {
            sb.Append((i+1).ToString());
            sb.Append(". ");

            int s = (i < highScore.Count ? highScore[i] : 0);
            if (s > 0) sb.Append(s);

            if(i < k_MaxHighScore - 1)
                sb.AppendLine();
        }

        highScoresText.text = sb.ToString();
    }

    #endregion

    #region Utilities

    private void ClearGrid()
    {
        /*
        var grid = TetrisGrid.Instance;

        for (int y = 0 ; y < grid.height; y++)
            for (int x = 0; x <  grid.width; x++)
                if (grid.grid[x,y] != null)
                {
                    Destroy(grid.grid[x, y].gameObject);
                    grid.grid[x, y] != null;
                }

        foreach (var tet in FindObjectsByType<Tetromino>(FindObjectsSortMode.None))
            Destroy(tet.gameObject);
        spawner.ClearPreview();*/
    }

    #endregion

    #region Sound Effects

    public void PlayNegativeBeep()
    {
        sfxPlayer.PlayOneShot(negativeBeep);
    }

    public void PlayRotateBlock()
    {
        sfxPlayer.PlayOneShot(rotateBlock);
    }

    public void PlayBlockPlaced()
    {
        sfxPlayer.PlayOneShot(placeBlock);
    }

    public void PlayLineCleared()
    {
        sfxPlayer.PlayOneShot(lineClear);
    }

    public void PlayLevelUp()
    {
        sfxPlayer.PlayOneShot(levelUp);
    }

    #endregion



}