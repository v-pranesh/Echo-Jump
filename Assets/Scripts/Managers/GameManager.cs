//GamerManager.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public int currentLevel = 1;
    public int totalLevels = 3;
    public int score = 0;
    
    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string gameOverScene = "GameOver";
    public string victoryScene = "Victory";
    
    private bool isPaused = false;
    private bool gameEnded = false;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        Time.timeScale = 1f;
    }
    
    void Update()
    {
        // Pause menu toggle
        if (Input.GetKeyDown(KeyCode.Escape) && !gameEnded)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    
    public void StartGame()
    {
        score = 0;
        currentLevel = 1;
        gameEnded = false;
        LoadLevel(1);
    }
    
    public void LoadLevel(int levelNumber)
    {
        currentLevel = levelNumber;
        string sceneName = "Level" + levelNumber;
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
        isPaused = false;
    }
    
    public void CompleteLevel()
    {
        if (currentLevel >= totalLevels)
        {
            // Game completed
            Victory();
        }
        else
        {
            // Load next level
            LoadLevel(currentLevel + 1);
        }
    }
    
    public void GameOver()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }
    
    public void Victory()
    {
        gameEnded = true;
        SceneManager.LoadScene(victoryScene);
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        UIManager.Instance?.ShowPauseMenu();
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance?.HidePauseMenu();
    }

    public void RestartLevel()
    {
        Debug.Log("🔄 RestartLevel called!");
        currentLevel = 1;
        SceneManager.LoadScene("Level1");
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void AddScore(int points)
    {
        score += points;
        UIManager.Instance?.UpdateScore(score);
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
}