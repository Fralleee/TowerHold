using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Upgrade/Health Regeneration")]
public class UpgradeHealthRegeneration : DefenseShopItem
{
	[Header("Upgrade Settings")]
	public int HealthRegenerationIncrease = 10;

	public override void OnPurchase()
	{
		base.OnPurchase();

		Tower.Instance.HealthRegenerationRate += HealthRegenerationIncrease;
		ScoreManager.Instance.Upgrades += 1;
	}
}
