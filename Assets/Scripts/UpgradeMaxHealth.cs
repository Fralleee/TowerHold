using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Upgrade Max Health")]
public class UpgradeMaxHealth : ShopItem
{
	[Header("Upgrade Settings")]
	public int HealthIncrease = 500;

	public override void OnPurchase()
	{
		Tower.Instance.UpgradeHealth(HealthIncrease);
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
