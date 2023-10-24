using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Upgrade")]
public class Upgrade : ShopItem
{
    [Header("Upgrade Settings")]
    public DamageType damageType;
    float increaseDamageFactor = 1.1f;

    public float GetDamage(float baseDamage)
    {
        return baseDamage * increaseDamageFactor;
    }

    public void Level()
    {
        increaseDamageFactor += 0.1f;
    }

    public override void OnPurchase()
    {
        Tower.instance.AddUppgrade(damageType);
    }
}
