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
		{ DamageType.Force, null },
		{ DamageType.Precision, null },
		{ DamageType.Technology, null },
		{ DamageType.Chemical, null }
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
		{ DamageType.Force, Color.white },
		{ DamageType.Precision, Color.black },
		{ DamageType.Technology, Color.cyan },
		{ DamageType.Chemical, Color.magenta }
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
			shopColor = DamageTypeColors[damageItem.Category];
		}
		else
		{
			Debug.LogError("No UI values found for item: " + item);
		}

		return shopColor;
	}
}
