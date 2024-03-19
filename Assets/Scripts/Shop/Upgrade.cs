using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Upgrade")]
public class Upgrade : DamageShopItem
{
	[Header("Upgrade Settings")]
	float _increaseDamageFactor = 1.1f;

	public float GetDamage(float baseDamage)
	{
		return baseDamage * _increaseDamageFactor;
	}

	public void Level()
	{
		_increaseDamageFactor += 0.1f;
	}

	public override void OnPurchase()
	{
		base.OnPurchase();

		Tower.Instance.AddUppgrade(Category);
		ScoreManager.Instance.Upgrades += 1;
	}
}
