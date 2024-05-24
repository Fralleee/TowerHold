using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : Singleton<GameController>
{
	public static GameSettings GameSettings => Instance.Settings;

	[HideInInspector] public RandomGenerator RandomGenerator;

	[ReadOnly] public GameState CurrentState;
	[HideInPlayMode] public GameState StartState = GameState.Idle;
	[InlineEditor(InlineEditorModes.GUIOnly)] public GameSettings Settings;

	EnemyManager _enemyManager;
	StateMachine<GameState> _stateMachine;
	IdleState _idleState;
	PreparationState _preparationState;
	LiveState _liveState;
	ConclusionState _conclusionState;

	public int CurrentLevel => _stateMachine.CurrentState is ILevelProgress stateWithProgress ? stateWithProgress.CurrentLevel : 0;
	public (float timeLeft, float totalTime, float progress) LevelProgress
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

	EventBinding<TowerDeathEvent> _towerDeathEvent;
	EventBinding<PreparationCompleteEvent> _preparationCompleteEvent;
	EventBinding<GameEndEvent> _gameEndEvent;

	void OnEnable()
	{
		_preparationCompleteEvent = new EventBinding<PreparationCompleteEvent>(e => _stateMachine.SetState(_liveState));
		EventBus<PreparationCompleteEvent>.Register(_preparationCompleteEvent);

		_towerDeathEvent = new EventBinding<TowerDeathEvent>(e => _stateMachine.SetState(_conclusionState));
		EventBus<TowerDeathEvent>.Register(_towerDeathEvent);

		_gameEndEvent = new EventBinding<GameEndEvent>(e => _stateMachine.SetState(_conclusionState));
		EventBus<GameEndEvent>.Register(_gameEndEvent);
	}

	void OnDisable()
	{
		EventBus<PreparationCompleteEvent>.Deregister(_preparationCompleteEvent);
		EventBus<TowerDeathEvent>.Deregister(_towerDeathEvent);
		EventBus<GameEndEvent>.Deregister(_gameEndEvent);
	}

	protected override void Awake()
	{
		base.Awake();

		Application.runInBackground = true;

		_enemyManager = GetComponentInChildren<EnemyManager>();

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
		_stateMachine.OnTransition += (IState<GameState> newState) => CurrentState = newState.Identifier;

		_idleState = new IdleState();
		_preparationState = new PreparationState();
		_liveState = new LiveState(_enemyManager);
		_conclusionState = new ConclusionState(_enemyManager);

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

		RandomGenerator = new RandomGenerator(Settings.StartSeed);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Time.timeScale = 1;

		Tower.ResetGameState();
		Enemy.ResetGameState();
	}
}
