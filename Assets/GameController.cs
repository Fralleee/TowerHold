using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float freezeTime = 5f;
    public float levelTime = 30f;
    public int startLevel = 1;
    public int maxLevel = 100;

    EnemySpawner enemySpawner;
    int currentLevel = 1;

    void Awake()
    {
        enemySpawner = GetComponentInChildren<EnemySpawner>();
    }

    void Start()
    {
        currentLevel = startLevel;
        Invoke("StartGame", freezeTime);
    }

    IEnumerator LevelController()
    {
        while (Tower.instance.health > 0 && currentLevel <= maxLevel)
        {
            yield return new WaitForSeconds(levelTime);
            currentLevel++;
            enemySpawner.spawnRate = Mathf.Max(0.1f, enemySpawner.spawnRate - 0.1f * currentLevel / 10);
        }

        EndGame();
    }

    void StartGame()
    {
        StartSpawning();
        StartCoroutine(LevelController());
    }

    void StartSpawning()
    {
        enemySpawner.target = Tower.instance;
        enemySpawner.IsSpawning = true;
    }

    void EndGame()
    {
        Debug.Log("Game Over! Your score is: " + CalculateScore());
    }

    int CalculateScore()
    {
        // Boilerplate scoring logic: level * remaining health * base multiplier
        int baseMultiplier = 10;
        return currentLevel * Tower.instance.health * baseMultiplier;
    }

    public void ReplayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
