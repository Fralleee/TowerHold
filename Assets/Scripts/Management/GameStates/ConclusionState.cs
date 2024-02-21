public class ConclusionState : IState<GameState>
{
	public GameState Identifier => GameState.Conclusion;

	readonly EnemyManager _enemyManager;

	public ConclusionState(EnemyManager enemyManager)
	{
		_enemyManager = enemyManager;
	}

	public void OnEnter()
	{
		_enemyManager.IsSpawning = false;
		Enemy.GameOver();

		GameController.OnGameEnd();
	}

	public void OnLogic()
	{
		// Do something
	}

	public void OnExit()
	{
		// Do something
	}
}
