using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Upgrade/Max Health")]
public class UpgradeMaxHealth : DefenseShopItem
{
	[Header("Upgrade Settings")]
	public int HealthIncrease = 500;

	public override void OnPurchase()
	{
		base.OnPurchase();

		Tower.Instance.UpgradeHealth(HealthIncrease);
		ScoreManager.Instance.Upgrades += 1;
	}
}
