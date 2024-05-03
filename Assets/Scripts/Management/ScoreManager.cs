using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
	[Header("References")]
	[SerializeField] ScoreUI _scoreUI;

	[Header("Scores")]
	public float DamageDone = 0;
	public float DamageTaken = 0;
	public int EnemiesKilled = 0;

	public int ResourcesSpent = 0;
	public int ResourcesEarned = 0;

	public int Turrets = 0;
	public int Upgrades = 0;

	EventBinding<EnemyDeathEvent> _enemyDeathEvent;
	EventBinding<GameEndEvent> _gameEndEvent;

	void OnEnable()
	{
		_enemyDeathEvent = new EventBinding<EnemyDeathEvent>(HandleEnemyDeath);
		EventBus<EnemyDeathEvent>.Register(_enemyDeathEvent);

		_gameEndEvent = new EventBinding<GameEndEvent>(HandleGameEnd);
		EventBus<GameEndEvent>.Register(_gameEndEvent);
	}

	void OnDisable()
	{
		EventBus<EnemyDeathEvent>.Deregister(_enemyDeathEvent);
		EventBus<GameEndEvent>.Deregister(_gameEndEvent);
	}

	protected override void Awake()
	{
		base.Awake();

		_scoreUI.gameObject.SetActive(false);
	}

	void HandleEnemyDeath(EnemyDeathEvent e)
	{
		EnemiesKilled += 1;
	}

	public string GetScoresAsText()
	{
		var scores = "Damage Done: " + DamageDone + "\n";
		scores += "Damage Taken: " + DamageTaken + "\n";
		scores += "Enemies Killed: " + EnemiesKilled + "\n";
		scores += "Resources Spent: " + ResourcesSpent + "\n";
		scores += "Resources Earned: " + ResourcesEarned + "\n";
		scores += "Turrets: " + Turrets + "\n";
		scores += "Upgrades: " + Upgrades + "\n";
		return scores;
	}

	void HandleGameEnd()
	{
		_scoreUI.gameObject.SetActive(true);
		_scoreUI.SetScores(GetScoresAsText());
	}
}
