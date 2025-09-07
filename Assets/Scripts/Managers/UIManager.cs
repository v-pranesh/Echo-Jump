using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("Health UI")]
    public Transform healthContainer;
    public GameObject heartPrefab;
    
    [Header("Game UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    
    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button pauseQuitButton;
    
    [Header("Main Menu")]
    public Button playButton;
    public Button mainMenuQuitButton;
    
    [Header("Game Over")]
    public Button restartButton;
    public Button gameOverQuitButton;
    
    [Header("Victory")]
    public Button victoryRestartButton;
    public Button victoryQuitButton;
    
    private Image[] healthHearts;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        SetupButtons();
        
        // DEBUG: Check if player exists
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("UIManager: Player not found! Make sure player has 'Player' tag.");
            return;
        }
        
        // DEBUG: Check if health system exists
        HealthSystem healthSystem = player.GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("UIManager: HealthSystem not found on player!");
            return;
        }
        
        Debug.Log("UIManager: Player and HealthSystem found! Setting up health UI...");
        
        SetupHealthUI();
        UpdateLevelText();
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }
    
    void SetupButtons()
    {
        // Main Menu buttons
        if (playButton != null)
            playButton.onClick.AddListener(() => GameManager.Instance?.StartGame());
        
        if (mainMenuQuitButton != null)
            mainMenuQuitButton.onClick.AddListener(() => GameManager.Instance?.QuitGame());
        
        // Pause Menu buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => GameManager.Instance?.ResumeGame());
        
        if (pauseQuitButton != null)
            pauseQuitButton.onClick.AddListener(() => GameManager.Instance?.LoadMainMenu());
        
        // Game Over buttons
        if (restartButton != null)
            restartButton.onClick.AddListener(() => GameManager.Instance?.RestartLevel());
        
        if (gameOverQuitButton != null)
            gameOverQuitButton.onClick.AddListener(() => GameManager.Instance?.LoadMainMenu());
        
        // Victory buttons
        if (victoryRestartButton != null)
            victoryRestartButton.onClick.AddListener(() => GameManager.Instance?.StartGame());
        
        if (victoryQuitButton != null)
            victoryQuitButton.onClick.AddListener(() => GameManager.Instance?.LoadMainMenu());
    }
    
    void SetupHealthUI()
    {
        if (healthContainer == null)
        {
            Debug.LogError("UIManager: Health Container not assigned!");
            return;
        }
        
        if (heartPrefab == null)
        {
            Debug.LogError("UIManager: Heart Prefab not assigned!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            HealthSystem healthSystem = player.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                CreateHealthHearts(healthSystem.maxHealth);
                healthSystem.OnHealthChanged.AddListener(UpdateHealthUI);
                Debug.Log("UIManager: Health UI setup complete! Hearts: " + healthSystem.maxHealth);
            }
            else
            {
                Debug.LogError("UIManager: HealthSystem component missing on player!");
            }
        }
        else
        {
            Debug.LogError("UIManager: Player object not found for health setup!");
        }
    }
    
    void CreateHealthHearts(int maxHealth)
    {
        healthHearts = new Image[maxHealth];
        
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, healthContainer);
            healthHearts[i] = heart.GetComponent<Image>();
        }
    }
    
    public void UpdateHealthUI(int currentHealth)
    {
        Debug.Log("UIManager: Updating health UI - " + currentHealth + " hearts");
        
        if (healthHearts == null)
        {
            Debug.LogError("UIManager: Health hearts array is null!");
            return;
        }
        
        for (int i = 0; i < healthHearts.Length; i++)
        {
            if (healthHearts[i] != null)
            {
                healthHearts[i].color = i < currentHealth ? Color.white : Color.gray;
            }
        }
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
    
    void UpdateLevelText()
    {
        if (levelText != null && GameManager.Instance != null)
            levelText.text = "Level " + GameManager.Instance.currentLevel;
    }
    
    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }
    
    public void HidePauseMenu()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }
    
    public void OnLevelComplete()
    {
        Invoke(nameof(LoadNextLevel), 2f);
    }
    
    void LoadNextLevel()
    {
        GameManager.Instance?.CompleteLevel();
    }
}