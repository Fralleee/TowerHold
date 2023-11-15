using System;
using UnityEngine;
public class ShopItem : ScriptableObject
{
    [Header("Shop Settings")]
    public string itemName;
    public Sprite image;
    public Category category;
    public int cost;
    public int minLevel;

    public virtual void OnPurchase()
    {
        throw new NotImplementedException();
    }
}
