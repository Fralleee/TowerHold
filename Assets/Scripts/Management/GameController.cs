using System;
using Sirenix.OdinInspector;
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

	[ReadOnly] public GameState CurrentState;
	public GameState StartState = GameState.Idle;
	public GameSettings Settings;

	EnemyManager _enemyManager;
	StateMachine<GameState> _stateMachine;

	IdleState _idleState;
	PreparationState _preparationState;
	LiveState _liveState;
	ConclusionState _conclusionState;

	public int CurrentLevel => _stateMachine.CurrentState is ILevelProgress stateWithProgress ? stateWithProgress.CurrentLevel : 0;

	public (float timeLeft, float totalTime, float progress) Progress
	{
		get
		{
			if (_stateMachine.CurrentState is ILevelProgress stateWithProgress)
			{
				return (stateWithProgress.TimeLeft, stateWithProgress.TotalTime, stateWithProgress.LevelProgress);
			}
			return (0f, 0f, 0f);
		}
	}

	protected override void Awake()
	{
		base.Awake();

		_enemyManager = GetComponentInChildren<EnemyManager>();
		RandomGenerator = new RandomGenerator(Settings.StartSeed);

		Tower.OnTowerDeath += OnTowerDeath;

		InitializeStateMachine();
		InitializeGameSettings();

		Debug.Log($"Starting game in {Settings.FreezeTime} seconds | Seed: {Settings.StartSeed} | Level: {Settings.StartLevel} | Map: {NameGeneration.GenerateLevelName(Settings.StartSeed)}");
	}

	void FixedUpdate()
	{
		_stateMachine.OnLogic();
	}

	void InitializeStateMachine()
	{
		_stateMachine = new StateMachine<GameState>();
		_stateMachine.OnTransition += OnTransition;

		_idleState = new IdleState();
		_preparationState = new PreparationState();
		_liveState = new LiveState(_enemyManager);
		_conclusionState = new ConclusionState(_enemyManager);

		_preparationState.OnPreparationComplete += () => _stateMachine.SetState(_liveState);
		_liveState.OnMaxLevelReached += () => _stateMachine.SetState(_conclusionState);

		switch (StartState)
		{
			case GameState.Idle:
				_stateMachine.SetState(_idleState);
				break;
			case GameState.Preparation:
				_stateMachine.SetState(_preparationState);
				break;
			case GameState.Live:
				_stateMachine.SetState(_liveState);
				break;
			case GameState.Conclusion:
				_stateMachine.SetState(_conclusionState);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	void InitializeGameSettings()
	{
		if (Settings.StartSeed == 0)
		{
			Settings.StartSeed = Random.Range(0, int.MaxValue);
		}
	}


	void OnTransition(IState<GameState> newState)
	{
		CurrentState = newState.Identifier;
	}

	public void ReplayGame() => SceneManager.LoadScene("Game");

	public void Menu() => SceneManager.LoadScene("Menu");

	protected override void OnDestroy()
	{
		base.OnDestroy();

		OnGameStart = delegate
		{ };
		OnLevelChanged = delegate
		{ };
		OnGameEnd = delegate
		{ };

		Time.timeScale = 1;

		Tower.OnTowerDeath -= OnTowerDeath;
		Tower.ResetGameState();
		Enemy.ResetGameState();
	}

	void OnTowerDeath()
	{
		_stateMachine.SetState(_conclusionState);
	}
}
