using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : Singleton<GameController>
{
	[HideInInspector]
	public RandomGenerator RandomGenerator;

	public static Action OnLevelChanged = delegate { };
	public static Action OnGameEnd = delegate { };
	public float FreezeTime = 5f;
	public float TimePerLevel = 10f;
	public float TimeLeft;
	public int StartLevel = 1;
	public int CurrentLevel = 1;
	public int MaxLevel = 100;
	public int StartSeed;

	bool _gameHasStarted = false;
	bool _gameHasEnded = false;

	EnemySpawner _enemySpawner;

	protected override void Awake()
	{
		base.Awake();

		if (StartSeed == 0)
		{
			StartSeed = Random.Range(0, int.MaxValue);
		}

		Debug.Log($"Starting game in {FreezeTime} seconds | Seed: {StartSeed} | Level: {StartLevel} | Map: {NameGeneration.GenerateLevelName(StartSeed)}");
	}

	void Start()
	{
		_enemySpawner = GetComponentInChildren<EnemySpawner>();
		RandomGenerator = new RandomGenerator(StartSeed);

		Invoke(nameof(StartGame), FreezeTime);
	}

	void Update()
	{
		if (_gameHasEnded || !_gameHasStarted)
		{
			return;
		}

		if (Tower.Instance.Health <= 0 || CurrentLevel > MaxLevel)
		{
			EndGame();
			return;
		}

		TimeLeft -= Time.deltaTime;

		if (TimeLeft <= 0)
		{
			RunLevel(CurrentLevel + 1);
		}
	}

	void RunLevel(int level)
	{
		CurrentLevel = level;
		TimeLeft += TimePerLevel;

		OnLevelChanged();
	}

	void StartGame()
	{
		RunLevel(StartLevel);

		_enemySpawner.Target = Tower.Instance;
		_enemySpawner.IsSpawning = true;

		_gameHasStarted = true;
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
