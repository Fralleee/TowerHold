public class ResourceShopItem : ShopItem
{
	public ResourceType Category;

	void OnValidate()
	{
		ShopType = ShopType.Income;
	}
}
