using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "StyleSettings", menuName = "VAKT/Shop/StyleSettings")]
public class StyleSettings : SerializedScriptableObject
{
	public Dictionary<RarityType, Color> RarityColors = new Dictionary<RarityType, Color> {
		{ RarityType.Common, new Color(0.8f, 0.8f, 0.8f) },
		{ RarityType.Uncommon, new Color(0.2f, 0.8f, 0.2f) },
		{ RarityType.Rare, new Color(0.2f, 0.2f, 0.8f) },
		{ RarityType.Epic, new Color(0.8f, 0.2f, 0.8f) },
		{ RarityType.Legendary, new Color(0.8f, 0.8f, 0.2f) }
	};

	public Dictionary<ShopType, Color> ShopTypeColors = new Dictionary<ShopType, Color> {
		{ ShopType.Income, new Color(0.8f, 0.8f, 0.8f) },
		{ ShopType.Defense, new Color(0.2f, 0.8f, 0.2f) },
		{ ShopType.Arcane, new Color(0.2f, 0.2f, 0.8f) },
		{ ShopType.Normal, new Color(0.8f, 0.2f, 0.8f) },
		{ ShopType.Siege, new Color(0.8f, 0.8f, 0.2f) },
		{ ShopType.Technology, new Color(0.8f, 0.8f, 0.8f) },
		{ ShopType.Void, new Color(0.2f, 0.8f, 0.2f) }
	};

	public Dictionary<ShopType, Texture2D> ShopTypeIcons = new Dictionary<ShopType, Texture2D> {
		{ ShopType.Income, null },
		{ ShopType.Defense, null },
		{ ShopType.Arcane, null },
		{ ShopType.Normal, null },
		{ ShopType.Siege, null },
		{ ShopType.Technology, null },
		{ ShopType.Void, null }
	};
}
