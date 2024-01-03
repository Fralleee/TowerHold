using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Defense")]
public class DefenseShopItem : ShopItem
{
	public DefenseType Category;

	void OnValidate()
	{
		ShopType = ShopType.Defense;
	}
}
