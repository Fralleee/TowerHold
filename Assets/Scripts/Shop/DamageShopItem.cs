using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Damage")]
public class DamageShopItem : ShopItem
{
	public DamageType Category;

	void OnValidate()
	{
		ShopType = Category switch
		{
			DamageType.Normal => ShopType.Normal,
			DamageType.Siege => ShopType.Siege,
			DamageType.Technology => ShopType.Technology,
			DamageType.Arcane => ShopType.Arcane,
			DamageType.Void => ShopType.Void,
			_ => throw new System.NotImplementedException()
		};
	}
}
