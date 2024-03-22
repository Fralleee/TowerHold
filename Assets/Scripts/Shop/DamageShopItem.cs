public class DamageShopItem : ShopItem
{
	public DamageType Category;

	void OnValidate()
	{
		ShopType = Category switch
		{
			DamageType.Power => ShopType.Power,
			DamageType.Precision => ShopType.Precision,
			DamageType.Technology => ShopType.Technology,
			DamageType.Arcane => ShopType.Arcane,
			DamageType.Chemical => ShopType.Chemical,
			_ => throw new System.NotImplementedException()
		};
	}
}
