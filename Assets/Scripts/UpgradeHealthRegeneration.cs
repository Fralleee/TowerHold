using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Upgrade Health Regeneration")]
public class UpgradeHealthRegeneration : ShopItem
{
  [Header("Upgrade Settings")]
  public int HealthRegenerationIncrease = 10;

  public override void OnPurchase()
  {
    Tower.instance.HealthRegenerationRate += HealthRegenerationIncrease;
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
