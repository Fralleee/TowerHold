public enum ResourceType
{
	Income
}

public enum DefenseType
{
	Health,
	Defense
}

public enum DamageType
{
	Force,
	Precision,
	Technology,
	Arcane,
	Chemical,
	All
}

public static class DamageTypeExtensions
{
	public static ShopType AsShopType(this DamageType damageType)
	{
		return damageType switch
		{
			DamageType.Force => ShopType.Force,
			DamageType.Precision => ShopType.Precision,
			DamageType.Technology => ShopType.Technology,
			DamageType.Arcane => ShopType.Arcane,
			DamageType.Chemical => ShopType.Chemical,
			DamageType.All => ShopType.Offense,
			_ => ShopType.Unspecified
		};
	}
}

public enum ShopType
{
	Unspecified,
	Income,
	Offense,
	Defense,
	Force,
	Precision,
	Technology,
	Arcane,
	Chemical
}
