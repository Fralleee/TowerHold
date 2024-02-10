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

	protected override void Awake()
	{
		base.Awake();

		_scoreUI.gameObject.SetActive(false);
		Enemy.OnAnyDeath += HandleEnemyDeath;
		GameController.OnGameEnd += HandleGameEnd;
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

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Enemy.OnAnyDeath -= HandleEnemyDeath;
		GameController.OnGameEnd -= HandleGameEnd;
	}
}
