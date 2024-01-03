using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Available Items")]
public class ShopInventory : ScriptableObject
{
	public ShopItem[] Items;

#if UNITY_EDITOR
	[Button("Populate all items")]
	public void PopulateAllItems()
	{
		var itemList = new List<ShopItem>();

		// Find all assets of type ShopItem
		var guids = AssetDatabase.FindAssets("t:ShopItem");
		foreach (var guid in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var item = AssetDatabase.LoadAssetAtPath<ShopItem>(path);
			if (item != null)
			{
				itemList.Add(item);
			}
		}

		Items = itemList.ToArray();
	}
#endif

#if UNITY_EDITOR
	[Button("Populate weapons and upgrades")]
	public void PopulateWeaponsAndUpgrades()
	{
		var itemList = new List<ShopItem>();

		// Find all assets of type ShopItem
		var guids = AssetDatabase.FindAssets("t:DamageShopItem");
		foreach (var guid in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var item = AssetDatabase.LoadAssetAtPath<DamageShopItem>(path);
			if (item != null)
			{
				itemList.Add(item);
			}
		}

		Items = itemList.ToArray();
	}
#endif
}

