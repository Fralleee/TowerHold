using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Upgrade/Max Health")]
public class UpgradeMaxHealth : DefenseShopItem
{
	[Header("Upgrade Settings")]
	public int HealthIncrease = 500;

	public override void OnPurchase()
	{
		Tower.Instance.UpgradeHealth(HealthIncrease);
		ScoreManager.Instance.Upgrades += 1;
	}
}
