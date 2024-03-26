using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Upgrade/Health Regeneration")]
public class UpgradeHealthRegeneration : DefenseShopItem
{
	public void Reset()
	{
		Amount = 10f;
		Description = "Increases health regeneration by {Amount}{TypeIcon}.";
	}

	public override void OnPurchase()
	{
		base.OnPurchase();

		Tower.Instance.HealthRegenerationRate += (int)Amount;
		ScoreManager.Instance.Upgrades += 1;
	}
}
