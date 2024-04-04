using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Upgrade")]
public class DamageUpgrade : DamageShopItem
{
	public void Reset()
	{
		Amount = 0.1f;
		Description = "Increases {Type} weapons by {PercentType}.";
	}

	public override void OnPurchase()
	{
		base.OnPurchase();

		Tower.Instance.AddUppgrade(this);
		ScoreManager.Instance.Upgrades += 1;
	}
}
