using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : Singleton<GameController>
{
	public static GameSettings GameSettings => Instance.Settings;

	public static Action OnGameStart = delegate { };
	public static Action<int> OnLevelChanged = delegate { };
	public static Action OnGameEnd = delegate { };

	[HideInInspector] public RandomGenerator RandomGenerator;
	[HideInInspector] public int CurrentLevel = 1;
	[HideInInspector] public float FreezeTimeLeft;
	[HideInInspector] public float TimeLeft;
	[HideInInspector] public bool GameHasStarted = false;
	[HideInInspector] public bool GameHasEnded = false;

	public GameSettings Settings;

	EnemySpawner _enemySpawner;

	public float LevelProgress => GameHasStarted ? (TimeLeft / Settings.TimePerLevel) : (FreezeTimeLeft / Settings.FreezeTime);

	protected override void Awake()
	{
		base.Awake();

		if (Settings.StartSeed == 0)
		{
			Settings.StartSeed = Random.Range(0, int.MaxValue);
		}

		_enemySpawner = GetComponentInChildren<EnemySpawner>();
		RandomGenerator = new RandomGenerator(Settings.StartSeed);

		Debug.Log($"Starting game in {Settings.FreezeTime} seconds | Seed: {Settings.StartSeed} | Level: {Settings.StartLevel} | Map: {NameGeneration.GenerateLevelName(Settings.StartSeed)}");
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
		if (level > Settings.MaxLevel)
		{
			EndGame();
			return;
		}

		CurrentLevel = level;
		TimeLeft += Settings.TimePerLevel;

		OnLevelChanged(CurrentLevel);
	}

	void StartGame()
	{
		RunLevel(Settings.StartLevel);

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
		FreezeTimeLeft = Settings.FreezeTime;
		TimeLeft = 0;
	}

	void OnTowerDeath(Target target)
	{
		EndGame();
	}
}
