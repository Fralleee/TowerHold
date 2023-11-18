using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Upgrade Health Regeneration")]
public class UpgradeHealthRegeneration : ShopItem
{
	[Header("Upgrade Settings")]
	public int HealthRegenerationIncrease = 10;

	public override void OnPurchase()
	{
		Tower.Instance.HealthRegenerationRate += HealthRegenerationIncrease;
		ScoreManager.Instance.Upgrades += 1;
	}

	void OnValidate()
	{
		if (Category != Category.Health)
		{
			Debug.LogWarning("Invalid category. Resetting to Health.");
			Category = Category.Health;
		}
	}
}
