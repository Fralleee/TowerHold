using UnityEngine;

public class PreparationState : IState<GameState>, ILevelProgress
{
	public GameState Identifier => GameState.Preparation;

	public int CurrentLevel => -1;
	public float TimeLeft { get; private set; }
	public float TotalTime => GameController.GameSettings.FreezeTime;
	public float LevelProgress => TimeLeft / TotalTime;


	public void OnEnter()
	{
		TimeLeft = GameController.GameSettings.FreezeTime;
	}

	public void OnUpdate()
	{
		TimeLeft -= Time.fixedDeltaTime;
		if (TimeLeft <= 0)
		{
			EventBus<PreparationCompleteEvent>.Raise(new PreparationCompleteEvent());
		}
	}

	public void OnExit()
	{

	}
}
