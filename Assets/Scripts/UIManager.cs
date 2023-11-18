using System.Collections.Generic;
using UnityEngine;

public class UIManager : SerializedSingleton<UIManager>
{
	public Dictionary<Category, Sprite> CategorySprites = new Dictionary<Category, Sprite>() {
		{ Category.Gold, null },
		{ Category.Health, null },
		{ Category.Armor, null },
		{ Category.Normal, null },
		{ Category.Piercing, null },
		{ Category.Siege, null },
		{ Category.Magic, null },
		{ Category.Chaos, null }
	};
	public Dictionary<Category, Color> CategoryColors = new Dictionary<Category, Color>(){
		{ Category.Gold, Color.yellow },
		{ Category.Health, Color.red },
		{ Category.Armor, Color.gray },
		{ Category.Normal, Color.white },
		{ Category.Piercing, Color.cyan },
		{ Category.Siege, Color.black },
		{ Category.Magic, Color.blue },
		{ Category.Chaos, Color.green }
	};
}
