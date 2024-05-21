using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "VAKT/GameSettings")]
public class GameSettings : ScriptableObject
{
	[Header("Game")]
	public float FreezeTime = 3f;
	public float TimePerLevel = 20f;
	public int StartLevel = 1;
	public int MaxLevel = 30;
	public int MaxEnemiesAlive = 60;
	public int StartSeed;

	[Header("Resources")]
	public int StartingResources = 5000;
	public int StartingIncome = 50;
}
