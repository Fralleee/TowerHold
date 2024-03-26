using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Income/Resource Generation")]
public class UpgradeResourceGeneration : ResourceShopItem
{
	public void Reset()
	{
		Amount = 10f;
		Description = "Increases income by {Amount}{TypeIcon}.";
	}

	public override void OnPurchase()
	{
		base.OnPurchase();

		ResourceManager.Instance.AddIncome((int)Amount);
		ScoreManager.Instance.Upgrades += 1;
	}
}
