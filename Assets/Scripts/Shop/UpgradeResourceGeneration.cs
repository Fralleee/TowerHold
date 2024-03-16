using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Income/Resource Generation")]
public class UpgradeResourceGeneration : ResourceShopItem
{
	[Header("Upgrade Settings")]
	public int ResourceGenerationIncrease = 10;

	public override void OnPurchase()
	{
		base.OnPurchase();

		ResourceManager.Instance.AddIncome(ResourceGenerationIncrease);
		ScoreManager.Instance.Upgrades += 1;
	}
}
