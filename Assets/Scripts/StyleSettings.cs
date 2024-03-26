using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "StyleSettings", menuName = "VAKT/Shop/StyleSettings")]
public class StyleSettings : SerializedScriptableObject
{
	[SerializeField]
	Dictionary<RarityType, Color> _rarityColors = new Dictionary<RarityType, Color> {
		{ RarityType.Common, new Color(0.8207547f, 0.8207547f, 0.8207547f) },
		{ RarityType.Uncommon, new Color(0.7129489f, 1f, 0.3066038f) },
		{ RarityType.Rare, new Color(0.2688679f, 0.497808278f, 1f) },
		{ RarityType.Epic, new Color(0.7286575f, 0.4292453f, 1f) },
		{ RarityType.Legendary, new Color(1f, 0.4571222f, 0.1745283f) }
	};

	[SerializeField]
	Dictionary<ShopType, Color> _shopTypeColors = new Dictionary<ShopType, Color> {
		{ ShopType.Income, new Color(0.967318356f, 0.4575472f, 1f) },
		{ ShopType.Defense, new Color(1f, 0.565647f, 0.5330188f) },
		{ ShopType.Arcane, new Color(0.5424528f, 0.6516403f, 1f) },
		{ ShopType.Force, new Color(0.9150943f, 0.9150943f, 0.9150943f) },
		{ ShopType.Precision, new Color(1f, 0.7747503f, 0.5330188f) },
		{ ShopType.Technology, new Color(0.5518868f, 0.9770199f, 1f) },
		{ ShopType.Chemical, new Color(0.539465368f, 1f, 0.5235849f) }
	};

	[SerializeField]
	Dictionary<ShopType, Texture2D> _shopTypeIcons = new Dictionary<ShopType, Texture2D> {
		{ ShopType.Income, null },
		{ ShopType.Defense, null },
		{ ShopType.Arcane, null },
		{ ShopType.Force, null },
		{ ShopType.Precision, null },
		{ ShopType.Technology, null },
		{ ShopType.Chemical, null }
	};

	[SerializeField]
	Dictionary<GameIcons, Texture2D> _icons = new Dictionary<GameIcons, Texture2D> {
		{ GameIcons.Cooldown, null },
		{ GameIcons.Gold, null },
	};

	public Texture2D GetShopTypeIcon(ShopType shopType)
	{
		return _shopTypeIcons[shopType];
	}

	public Color GetShopTypeColor(ShopType shopType)
	{
		if (_shopTypeColors.ContainsKey(shopType))
		{
			return _shopTypeColors[shopType];
		}

		Debug.LogError($"No color found for shop type {shopType}");
		return Color.white;
	}

	public Color GetShopTypeColorForImage(ShopType shopType)
	{
		if (shopType is ShopType.Income or ShopType.Defense or ShopType.Offense)
		{
			return Color.white;
		}

		if (_shopTypeColors.ContainsKey(shopType))
		{
			return _shopTypeColors[shopType].TintColor(0f);
		}

		Debug.LogError($"No color found for shop type {shopType}");
		return Color.white;
	}

	public Texture2D GetIcon(GameIcons icon)
	{
		if (_icons.ContainsKey(icon))
		{
			return _icons[icon];
		}

		Debug.LogError($"No icon found for {icon}");
		return null;
	}

	public Color GetRarityColor(RarityType rarityType)
	{
		if (_rarityColors.ContainsKey(rarityType))
		{
			return _rarityColors[rarityType];
		}

		Debug.LogError($"No color found for rarity type {rarityType}");
		return Color.white;
	}


	public Color GetRarityTintColor(RarityType rarityType)
	{
		if (_rarityColors.ContainsKey(rarityType))
		{
			return Color.Lerp(_rarityColors[rarityType], Color.white, 0.5f);
		}

		Debug.LogError($"No color found for rarity type {rarityType}");
		return Color.white;
	}
}
