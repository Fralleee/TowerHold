using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Upgrade")]
public class Upgrade : DamageShopItem
{
	[Header("Upgrade Settings")]
	float _increaseDamageFactor = 1.1f;

	public float GetDamage(float baseDamage) => baseDamage * _increaseDamageFactor;

	public void Level() => _increaseDamageFactor += 0.1f;

	public override void OnPurchase()
	{
		Tower.Instance.AddUppgrade(Category);
		ScoreManager.Instance.Upgrades += 1;
	}
}
