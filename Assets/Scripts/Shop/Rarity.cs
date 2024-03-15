using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RarityType
{
	Common,
	Uncommon,
	Rare,
	Epic,
	Legendary
}


public static class Rarity
{
	static readonly Dictionary<RarityType, float> BaseChances = new Dictionary<RarityType, float>
	{
		{ RarityType.Common, 60f },
		{ RarityType.Uncommon, 25f },
		{ RarityType.Rare, 15f },
		{ RarityType.Epic, 0f },
		{ RarityType.Legendary, 0f }
	};

	static readonly Dictionary<RarityType, float> EndChances = new Dictionary<RarityType, float>
	{
		{ RarityType.Common, 5f },
		{ RarityType.Uncommon, 10f },
		{ RarityType.Rare, 20f },
		{ RarityType.Epic, 30f },
		{ RarityType.Legendary, 35f }
	};

	public static RarityType SelectRarityBasedOnLevel(int currentLevel, int maxLevel, RandomGenerator randomGenerator)
	{
		var currentChances = BaseChances.ToDictionary(
			rarity => rarity.Key,
			rarity => Mathf.Lerp(
				BaseChances[rarity.Key],
				EndChances[rarity.Key],
				(float)currentLevel / maxLevel
			)
		);

		var totalChance = currentChances.Values.Sum();
		var randomChance = randomGenerator.NextFloat(0, totalChance);
		var cumulativeChance = 0f;

		foreach (var rarity in currentChances)
		{
			cumulativeChance += rarity.Value;
			if (randomChance <= cumulativeChance)
			{
				return rarity.Key;
			}
		}

		return RarityType.Common;
	}
}
