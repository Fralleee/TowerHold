using System;
using UnityEngine;

public class ShopItem : ScriptableObject
{
	[Header("Shop Settings")]
	public Sprite Image;
	public int Cost;
	public int MinLevel;

	public virtual void OnPurchase() => throw new NotImplementedException();
}
