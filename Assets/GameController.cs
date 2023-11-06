using System;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public static Action OnLevelChanged = delegate { };
    public static Action OnGameEnd = delegate { };
    public float freezeTime = 5f;
    public float levelTime = 30f;
    public float timeLeft;
    public int startLevel = 1;
    public int currentLevel = 1;
    public int maxLevel = 100;
    bool gameHasEnded = false;
    EnemySpawner enemySpawner;

    void Start()
    {
        enemySpawner = GetComponentInChildren<EnemySpawner>();
        currentLevel = startLevel;
        timeLeft = levelTime; // Initialize time left with level time
        Invoke(nameof(StartGame), freezeTime);
    }

    void Update()
    {
        if (gameHasEnded)
        {
            return;
        }

        if (Tower.instance?.health <= 0 || currentLevel > maxLevel)
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
        enemySpawner.spawnRate = Mathf.Max(0.1f, enemySpawner.spawnRate - 0.1f * currentLevel / 10);
        timeLeft = levelTime; // Reset the time left for the new level
        OnLevelChanged();
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
        Time.timeScale = 0;
        OnGameEnd();
    }

    public void ReplayGame()
    {
        ResetGameState();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void Menu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void ResetGameState()
    {
        OnLevelChanged = delegate { };
        OnGameEnd = delegate { };

        Time.timeScale = 1;
        gameHasEnded = false;

        GoldManager.ResetGameState();
        Enemy.ResetGameState();
        Tower.ResetGameState();
    }
}
