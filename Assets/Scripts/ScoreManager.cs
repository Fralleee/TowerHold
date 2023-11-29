using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
	[Header("References")]
	[SerializeField] ScoreUI _scoreUI;

	[Header("Scores")]
	public float DamageDone = 0; // Done
	public float DamageTaken = 0; // Done
	public int EnemiesKilled = 0; // Done

	public int ResourcesSpent = 0; // Done
	public int ResourcesEarned = 0; // Done

	public int Turrets = 0; // Done
	public int Upgrades = 0; // Done

	protected override void Awake()
	{
		base.Awake();

		_scoreUI.gameObject.SetActive(false);
	}

	void HandleEnemyDeath(Target enemy) => EnemiesKilled += 1;

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

	void OnEnable()
	{
		Enemy.OnAnyDeath += HandleEnemyDeath;
		GameController.OnGameEnd += HandleGameEnd;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Enemy.OnAnyDeath -= HandleEnemyDeath;
		GameController.OnGameEnd -= HandleGameEnd;
	}
}
