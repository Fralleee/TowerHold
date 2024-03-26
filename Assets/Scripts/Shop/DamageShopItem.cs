using Sirenix.OdinInspector;

public class DamageShopItem : ShopItem
{
	const string GetCategoryInfo =
			"Force (Normal + Siege):\n" +
			"- All-around category excelling in AOE within short ranges.\n" +
			"- Fast attack speed, versatile for dealing with groups.\n\n" +

			"Precision (Piercing):\n" +
			"- Inflicts massive damage at long distances to single targets.\n" +
			"- Critical hit capability acts as a tank killer but has slower attack rates.\n\n" +

			"Technology (Chaos):\n" +
			"- Defined by randomness, offering unpredictable outcomes.\n\n" +

			"Chemical (Poison + Fire):\n" +
			"- Masters of DOT, applying slow debuffs and increasing damage on previously attacked targets.\n" +
			"- Effective against tanky enemies by combining poison and fire.\n\n" +

			"Arcane (Magic):\n" +
			"- Focuses on utility with effects ranging from stuns to buffs.\n" +
			"- Essential for controlling battle flow and making strategic plays.";

	[DetailedInfoBox("Category Information", GetCategoryInfo, InfoMessageType.Info)]
	public DamageType Category;

	void OnValidate()
	{
		ShopType = Category switch
		{
			DamageType.Force => ShopType.Force,
			DamageType.Precision => ShopType.Precision,
			DamageType.Technology => ShopType.Technology,
			DamageType.Chemical => ShopType.Chemical,
			DamageType.Arcane => ShopType.Arcane,
			DamageType.All => ShopType.Offense,
			_ => throw new System.NotImplementedException()
		};
	}
}
