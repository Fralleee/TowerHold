using System;
using UnityEngine;

public class LiveState : IState<GameState>, ILevelProgress
{
	public event Action OnMaxLevelReached;
	public event Action OnTowerDeath;

	public GameState Identifier => GameState.Live;

	public int CurrentLevel { get; private set; }
	public float TimeLeft { get; private set; }
	public float TotalTime => GameController.GameSettings.TimePerLevel;
	public float LevelProgress => TimeLeft / TotalTime;

	readonly EnemyManager _enemyManager;

	public LiveState(EnemyManager enemyManager)
	{
		_enemyManager = enemyManager;
	}

	public void OnEnter()
	{
		_enemyManager.Target = Tower.Instance;
		_enemyManager.IsSpawning = true;
		GameController.OnGameStart();
		Tower.OnTowerDeath += OnTowerDeath;

		TimeLeft = 0;
		CurrentLevel = GameController.GameSettings.StartLevel;
		RunLevel(GameController.GameSettings.StartLevel);
	}

	public void OnUpdate()
	{
		TimeLeft -= Time.fixedDeltaTime;
		if (TimeLeft <= 0)
		{
			RunLevel(CurrentLevel + 1);
		}
	}

	public void OnExit()
	{
		Tower.OnTowerDeath -= OnTowerDeath;
	}

	void RunLevel(int level)
	{
		if (level > GameController.GameSettings.MaxLevel)
		{
			OnMaxLevelReached();
			return;
		}

		CurrentLevel = level;
		TimeLeft += GameController.GameSettings.TimePerLevel;

		GameController.OnLevelChanged(CurrentLevel);
	}
}
