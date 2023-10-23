using UnityEngine;

public class ShopItem : ScriptableObject
{
    [Header("Shop Settings")]
    public string itemName;
    public Sprite image;
    public int cost;
    public int minLevel; // Minimum level required for this item to appear in the shop

    public void OnPurchase()
    {
        // Implement your logic to apply the item
    }
}
