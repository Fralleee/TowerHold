using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Upgrade/Max Health")]
public class UpgradeMaxHealth : DefenseShopItem
{
	public void Reset()
	{
		Amount = 500f;
		Description = "Increases max health by {Amount}{TypeIcon}.";
	}

	public override void OnPurchase()
	{
		base.OnPurchase();

		Tower.Instance.UpgradeHealth((int)Amount);
		ScoreManager.Instance.Upgrades += 1;
	}
}
