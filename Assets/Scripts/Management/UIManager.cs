using System.Collections.Generic;
using UnityEngine;

public class UIManager : SerializedSingleton<UIManager>
{
	public Dictionary<ResourceType, Sprite> ResourceTypeSprites = new Dictionary<ResourceType, Sprite>() {
		{ ResourceType.Income, null }
	};
	public Dictionary<DefenseType, Sprite> DefenseTypeSprites = new Dictionary<DefenseType, Sprite>() {
		{ DefenseType.Defense, null },
		{ DefenseType.Health, null }
	};
	public Dictionary<ShopType, Sprite> DamageTypeSprites = new Dictionary<ShopType, Sprite>() {
		{ ShopType.Arcane, null },
		{ ShopType.Force, null },
		{ ShopType.Precision, null },
		{ ShopType.Technology, null },
		{ ShopType.Chemical, null }
	};

	public Dictionary<ResourceType, Color> ResourceTypeColors = new Dictionary<ResourceType, Color>() {
		{ ResourceType.Income, Color.yellow }
	};
	public Dictionary<DefenseType, Color> DefenseTypeColors = new Dictionary<DefenseType, Color>() {
		{ DefenseType.Defense, Color.gray },
		{ DefenseType.Health, Color.red }
	};
	public Dictionary<ShopType, Color> DamageTypeColors = new Dictionary<ShopType, Color>() {
		{ ShopType.Arcane, Color.blue },
		{ ShopType.Force, Color.white },
		{ ShopType.Precision, Color.black },
		{ ShopType.Technology, Color.cyan },
		{ ShopType.Chemical, Color.magenta }
	};

	public Color GetShopItemColor(ShopItem item)
	{
		var shopColor = Color.white;

		if (item is ResourceShopItem resourceItem)
		{
			shopColor = ResourceTypeColors[resourceItem.Category];
		}
		else if (item is DefenseShopItem defenseItem)
		{
			shopColor = DefenseTypeColors[defenseItem.Category];
		}
		else if (item is DamageShopItem damageItem)
		{
			shopColor = DamageTypeColors[damageItem.ShopType];
		}
		else
		{
			Debug.LogError("No UI values found for item: " + item);
		}

		return shopColor;
	}
}
