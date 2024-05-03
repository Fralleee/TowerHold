using UnityEngine;

public class LiveState : IState<GameState>, ILevelProgress
{
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

		EventBus<GameStartEvent>.Raise(new GameStartEvent());

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

	void RunLevel(int level)
	{
		if (level > GameController.GameSettings.MaxLevel)
		{
			EventBus<GameEndEvent>.Raise(new GameEndEvent());
			return;
		}

		CurrentLevel = level;
		TimeLeft += GameController.GameSettings.TimePerLevel;

		EventBus<LevelChangedEvent>.Raise(new LevelChangedEvent { CurrentLevel = CurrentLevel });
	}

	public void OnExit()
	{
	}
}
