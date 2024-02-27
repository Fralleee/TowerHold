using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopItem : ScriptableObject
{
	[HideInInspector] public ShopType ShopType;

	[Header("Shop Settings")]
	public Sprite Image;
	public RarityType RarityType;
	public int Cost
	{
		get
		{
			return RarityType switch
			{
				RarityType.Common => 500,
				RarityType.Uncommon => 2000,
				RarityType.Rare => 5000,
				RarityType.Epic => 10000,
				RarityType.Legendary => 25000,
				_ => throw new ArgumentOutOfRangeException(nameof(RarityType), RarityType, null),
			};
		}
	}

	public static ShopItem GetRandomItem(int currentLevel, ShopItem[] items, RandomGenerator randomGenerator)
	{
		var groupedItems = new Dictionary<ShopType, List<ShopItem>>();
		foreach (var item in items)
		{
			if (!groupedItems.ContainsKey(item.ShopType))
			{
				groupedItems[item.ShopType] = new List<ShopItem>();
			}
			groupedItems[item.ShopType].Add(item);
		}

		if (groupedItems.Count == 0)
		{
			return null;  // No items available at all
		}

		var maxAttempts = 15;  // Set a maximum number of attempts to prevent infinite loops
		var attempts = 0;

		while (attempts < maxAttempts)
		{
			var availableCategories = groupedItems.Keys.ToArray();
			var randomCategory = availableCategories[randomGenerator.Next(0, availableCategories.Length)];
			var itemsInCategory = groupedItems[randomCategory];

			var selectedRarity = Rarity.SelectRarityBasedOnLevel(currentLevel, GameController.GameSettings.MaxLevel, randomGenerator);
			var eligibleItems = itemsInCategory.Where(item => item.RarityType == selectedRarity).ToList();

			if (eligibleItems.Count > 0)
			{
				return eligibleItems[randomGenerator.Next(0, eligibleItems.Count)];
			}

			attempts++;
		}

		// If the loop exits due to reaching the maximum number of attempts,
		// return a random item from all available items or null if none are available
		var allItems = groupedItems.Values.SelectMany(x => x).ToList();
		if (allItems.Count > 0)
		{
			return allItems[randomGenerator.Next(0, allItems.Count)];
		}

		return null;  // Return null if there are no items at all
	}

	public virtual TooltipContent Tooltip(StyleSettings styleSettings)
	{
		var tooltip = new TooltipContent();
		tooltip.UpdateInformation(this, styleSettings);
		return tooltip;
	}

	public virtual void OnPurchase() => throw new NotImplementedException();
}
