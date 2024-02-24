public class DefenseShopItem : ShopItem
{
	public DefenseType Category;

	void OnValidate()
	{
		ShopType = ShopType.Defense;
	}
}
