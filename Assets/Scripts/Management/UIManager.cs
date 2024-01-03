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
	public Dictionary<DamageType, Sprite> DamageTypeSprites = new Dictionary<DamageType, Sprite>() {
		{ DamageType.Arcane, null },
		{ DamageType.Normal, null },
		{ DamageType.Siege, null },
		{ DamageType.Technology, null },
		{ DamageType.Void, null }
	};

	public Dictionary<ResourceType, Color> ResourceTypeColors = new Dictionary<ResourceType, Color>() {
		{ ResourceType.Income, Color.yellow }
	};
	public Dictionary<DefenseType, Color> DefenseTypeColors = new Dictionary<DefenseType, Color>() {
		{ DefenseType.Defense, Color.gray },
		{ DefenseType.Health, Color.red }
	};
	public Dictionary<DamageType, Color> DamageTypeColors = new Dictionary<DamageType, Color>() {
		{ DamageType.Arcane, Color.blue },
		{ DamageType.Normal, Color.white },
		{ DamageType.Siege, Color.black },
		{ DamageType.Technology, Color.cyan },
		{ DamageType.Void, Color.magenta }
	};

	public (Sprite, Color) GetShopItemUIValues(ShopItem item)
	{
		var shopImage = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
		var shopColor = Color.white;

		if (item is ResourceShopItem resourceItem)
		{
			shopImage = ResourceTypeSprites[resourceItem.Category];
			shopColor = ResourceTypeColors[resourceItem.Category];
		}
		else if (item is DefenseShopItem defenseItem)
		{
			shopImage = DefenseTypeSprites[defenseItem.Category];
			shopColor = DefenseTypeColors[defenseItem.Category];
		}
		else if (item is DamageShopItem damageItem)
		{
			shopImage = DamageTypeSprites[damageItem.Category];
			shopColor = DamageTypeColors[damageItem.Category];
		}
		else
		{
			Debug.LogError("No UI values found for item: " + item);
		}

		return (shopImage, shopColor);
	}
}
