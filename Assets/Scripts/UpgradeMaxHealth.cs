using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Upgrade Max Health")]
public class UpgradeMaxHealth : ShopItem
{
  [Header("Upgrade Settings")]
  public int HealthIncrease = 500;

  public override void OnPurchase()
  {
    Tower.instance.UpgradeHealth(HealthIncrease);
    ScoreManager.Instance.upgrades += 1;
  }

  void OnValidate()
  {
    if (category != Category.Health)
    {
      Debug.LogWarning("Invalid category. Resetting to Health.");
      category = Category.Health;
    }
  }
}
