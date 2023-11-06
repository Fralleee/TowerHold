using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Health
{
    public int healthRegenerationRate = 1;

    public static Tower instance;
    public List<Turret> turrets;
    public Dictionary<DamageType, float> damageMultipliers = new Dictionary<DamageType, float>() {
        { DamageType.Normal, 1f },
        { DamageType.Piercing, 1f },
        { DamageType.Siege, 1f },
        { DamageType.Magic, 1f },
        { DamageType.Poison, 1f },
        { DamageType.Chaos, 1f }
    };

    protected override void Awake()
    {
        base.Awake();

        instance = this;
        OnDamageTaken += HandleDamageTaken;
    }


    void Update()
    {
        RegenerateHealth();
        foreach (var turret in turrets)
        {
            turret.Update();
        }
    }

    private void RegenerateHealth()
    {
        // Increment health, ensuring that it doesn't exceed the maximum
        health = Mathf.Min(health + Mathf.RoundToInt(healthRegenerationRate * Time.deltaTime), maxHealth);

        // You may want to add a callback or event when the health changes, for UI updates or other game logic.
    }

    public void AddTurret(Turret turret)
    {
        var instance = Instantiate(turret);
        instance.Setup(this);
        turrets.Add(instance);
    }

    public void AddUppgrade(DamageType damageType)
    {
        damageMultipliers[damageType] += 0.1f;
    }

    public float GetDamage(DamageType damageType, float damage)
    {
        return damage * damageMultipliers[damageType];
    }

    void HandleDamageTaken(int damage)
    {
        ScoreManager.Instance.damageTaken += damage;
    }

    public static void ResetGameState()
    {
        instance = null;
        OnDeath = delegate { };
    }
}
