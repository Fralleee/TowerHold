using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : Singleton<GameController>
{
	public static Action OnGameStart = delegate { };
	public static Action OnLevelChanged = delegate { };
	public static Action OnGameEnd = delegate { };

	[HideInInspector] public RandomGenerator RandomGenerator;
	[HideInInspector] public float FreezeTimeLeft;
	[HideInInspector] public float TimeLeft;
	[HideInInspector] public bool GameHasStarted = false;
	[HideInInspector] public bool GameHasEnded = false;

	public float FreezeTime = 5f;
	public float TimePerLevel = 10f;
	public int StartLevel = 1;
	public int CurrentLevel = 1;
	public int MaxLevel = 100;
	public int StartSeed;

	EnemySpawner _enemySpawner;

	public float LevelProgress => GameHasStarted ? 1 - (TimeLeft / TimePerLevel) : 1 - (FreezeTimeLeft / FreezeTime);

	protected override void Awake()
	{
		base.Awake();

		if (StartSeed == 0)
		{
			StartSeed = Random.Range(0, int.MaxValue);
		}

		_enemySpawner = GetComponentInChildren<EnemySpawner>();
		RandomGenerator = new RandomGenerator(StartSeed);

		Debug.Log($"Starting game in {FreezeTime} seconds | Seed: {StartSeed} | Level: {StartLevel} | Map: {NameGeneration.GenerateLevelName(StartSeed)}");
	}

	void Start()
	{
		Tower.Instance.OnDeath += OnTowerDeath;
	}

	void FixedUpdate()
	{
		if (GameHasEnded)
		{
			return;
		}

		if (!GameHasStarted)
		{
			FreezeTimeLeft -= Time.deltaTime;
			if (FreezeTimeLeft <= 0)
			{
				StartGame();
			}
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
		if (level > MaxLevel)
		{
			EndGame();
			return;
		}

		CurrentLevel = level;
		TimeLeft += TimePerLevel;

		OnLevelChanged();
	}

	void StartGame()
	{
		RunLevel(StartLevel);

		_enemySpawner.Target = Tower.Instance;
		_enemySpawner.IsSpawning = true;

		GameHasStarted = true;
		OnGameStart();
	}

	void EndGame()
	{
		if (GameHasEnded)
		{
			return;
		}

		GameHasEnded = true;

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
		GameHasEnded = false;

		Tower.ResetGameState();
		Enemy.ResetGameState();
	}

	void OnValidate()
	{
		FreezeTimeLeft = FreezeTime;
		TimeLeft = 0;
	}

	void OnTowerDeath(Target target)
	{
		EndGame();
	}
}
