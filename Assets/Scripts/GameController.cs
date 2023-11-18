using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
	public static Action OnLevelChanged = delegate { };
	public static Action<GameStage> OnStageChanged = delegate { };
	public static Action OnGameEnd = delegate { };
	public float FreezeTime = 5f;
	public float LevelTime = 30f;
	public float TimeLeft;
	public int StartLevel = 1;
	public int CurrentLevel = 1;
	public int MaxLevel = 100;
	public GameStage CurrentStage;
	bool _gameHasEnded = false;
	EnemySpawner _enemySpawner;

	public GoldManagerSettings GoldManagerSettings;
	public EnemySpawnerSettings EnemySpawnerSettings;

	void Start()
	{
		_enemySpawner = GetComponentInChildren<EnemySpawner>();
		CurrentLevel = StartLevel;
		TimeLeft = LevelTime;
		UpdateGameStage();
		Invoke(nameof(StartGame), FreezeTime);
	}

	void Update()
	{
		if (_gameHasEnded)
		{
			return;
		}

		if (Tower.Instance.Health <= 0 || CurrentLevel > MaxLevel)
		{
			EndGame();
			return;
		}

		TimeLeft -= Time.deltaTime; // Decrease the time left by the time passed since last frame

		if (TimeLeft <= 0)
		{
			NextLevel();
		}
	}

	void NextLevel()
	{
		CurrentLevel++;
		UpdateGameStage();
		EnemySpawnerSettings.SpawnRate = Mathf.Max(0.1f, EnemySpawnerSettings.SpawnRate - (0.1f * CurrentLevel / 10));
		TimeLeft = LevelTime;
		OnLevelChanged();
	}

	void UpdateGameStage()
	{
		var previousStage = CurrentStage;

		var lowThreshold = (int)(MaxLevel * 0.4); // 40% of maxLevel
		var mediumThreshold = (int)(MaxLevel * 0.7); // 70% of maxLevel

		if (CurrentLevel <= lowThreshold)
		{
			CurrentStage = GameStage.Low;
		}
		else if (CurrentLevel <= mediumThreshold)
		{
			CurrentStage = GameStage.Medium;
		}
		else
		{
			CurrentStage = GameStage.High;
		}

		if (CurrentStage != previousStage)
		{
			OnStageChanged(CurrentStage);
		}
	}

	void StartGame() => StartSpawning();

	void StartSpawning()
	{
		_enemySpawner.Target = Tower.Instance;
		_enemySpawner.IsSpawning = true;
	}

	void EndGame()
	{
		if (_gameHasEnded)
		{
			return;
		}

		_gameHasEnded = true;

		_enemySpawner.IsSpawning = false;
		Enemy.GameOver();
		OnGameEnd();
	}

	public void ReplayGame() => SceneManager.LoadScene("Game");

	public void Menu() => SceneManager.LoadScene("Menu");

	protected override void OnDestroy()
	{
		base.OnDestroy();

		OnLevelChanged = delegate
		{ };
		OnGameEnd = delegate
		{ };

		Time.timeScale = 1;
		_gameHasEnded = false;

		Tower.ResetGameState();
		Enemy.ResetGameState();
	}
}
