using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : Singleton<GameController>
{
    public static Action OnLevelChanged = delegate { };
    public static Action<GameStage> OnStageChanged = delegate { };
    public static Action OnGameEnd = delegate { };
    public float freezeTime = 5f;
    public float levelTime = 30f;
    public float timeLeft;
    public int startLevel = 1;
    public int currentLevel = 1;
    public int maxLevel = 100;
    public GameStage currentStage;
    bool gameHasEnded = false;
    EnemySpawner enemySpawner;

    public GoldManagerSettings goldManagerSettings;
    public EnemySpawnerSettings enemySpawnerSettings;

    void Start()
    {
        enemySpawner = GetComponentInChildren<EnemySpawner>();
        currentLevel = startLevel;
        timeLeft = levelTime;
        UpdateGameStage();
        Invoke(nameof(StartGame), freezeTime);
    }

    void Update()
    {
        if (gameHasEnded)
        {
            return;
        }

        if (Tower.instance?.Health <= 0 || currentLevel > maxLevel)
        {
            EndGame();
            return;
        }

        timeLeft -= Time.deltaTime; // Decrease the time left by the time passed since last frame

        if (timeLeft <= 0)
        {
            NextLevel();
        }
    }

    void NextLevel()
    {
        currentLevel++;
        UpdateGameStage();
        enemySpawnerSettings.spawnRate = Mathf.Max(0.1f, enemySpawnerSettings.spawnRate - 0.1f * currentLevel / 10);
        timeLeft = levelTime;
        OnLevelChanged();
    }

    void UpdateGameStage()
    {
        GameStage previousStage = currentStage;

        int lowThreshold = (int)(maxLevel * 0.4); // 40% of maxLevel
        int mediumThreshold = (int)(maxLevel * 0.7); // 70% of maxLevel

        if (currentLevel <= lowThreshold)
            currentStage = GameStage.Low;
        else if (currentLevel <= mediumThreshold)
            currentStage = GameStage.Medium;
        else
            currentStage = GameStage.High;

        if (currentStage != previousStage)
            OnStageChanged(currentStage);
    }

    void StartGame()
    {
        StartSpawning();
    }

    void StartSpawning()
    {
        enemySpawner.target = Tower.instance;
        enemySpawner.IsSpawning = true;
    }

    void EndGame()
    {
        if (gameHasEnded)
        {
            return;
        }

        gameHasEnded = true;

        enemySpawner.IsSpawning = false;
        Enemy.GameOver();
        OnGameEnd();
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        OnLevelChanged = delegate { };
        OnGameEnd = delegate { };

        Time.timeScale = 1;
        gameHasEnded = false;

        Tower.ResetGameState();
        Enemy.ResetGameState();
    }
}
