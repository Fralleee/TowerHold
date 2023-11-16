using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Upgrade Gold Generation")]
public class UpgradeGoldGeneration : ShopItem
{
  [Header("Upgrade Settings")]
  public int GoldGenerationIncrease = 10;

  public override void OnPurchase()
  {
    GoldManager.Instance.IncomeRate += GoldGenerationIncrease;
    ScoreManager.Instance.upgrades += 1;
  }

  void OnValidate()
  {
    if (category != Category.Gold)
    {
      Debug.LogWarning("Invalid category. Resetting to Gold.");
      category = Category.Gold;
    }
  }
}