using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Resource")]
public class ResourceShopItem : ShopItem
{
	public ResourceType Category;

	void OnValidate()
	{
		ShopType = ShopType.Resource;
	}
}
