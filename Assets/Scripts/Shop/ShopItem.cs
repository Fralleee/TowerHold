using System;
using UnityEngine;

public class ShopItem : ScriptableObject
{
	[HideInInspector] public ShopType ShopType;

	[Header("Shop Settings")]
	public Sprite Image;
	public RarityType Rarity;

	public int Cost
	{
		get
		{
			return Rarity switch
			{
				RarityType.Common => 500,
				RarityType.Uncommon => 2000,
				RarityType.Rare => 5000,
				RarityType.Epic => 10000,
				RarityType.Legendary => 25000,
				_ => throw new ArgumentOutOfRangeException(nameof(Rarity), Rarity, null),
			};
		}
	}

	public virtual void OnPurchase() => throw new NotImplementedException();
}
