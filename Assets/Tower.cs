using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Health
{
    public int healthRegenerationRate = 5;

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
    }

    void Start()
    {
        StartCoroutine(RegenerateHealth());
    }


    void Update()
    {
        RegenerateHealth();
        foreach (var turret in turrets)
        {
            turret.Update();
        }
    }

    private IEnumerator RegenerateHealth()
    {
        while (true) // Creates an infinite loop, so the coroutine keeps running
        {
            // Increment health, ensuring that it doesn't exceed the maximum
            health = Mathf.Min(health + healthRegenerationRate, maxHealth);

            // You may want to add a callback or event when the health changes, for UI updates or other game logic.

            yield return new WaitForSeconds(1); // Wait for 1 second before the loop continues
        }
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

    void OnEnable()
    {
        OnDamageTaken += HandleDamageTaken;
    }

    void OnDestroy()
    {
        OnDamageTaken -= HandleDamageTaken;
    }

    public static void ResetGameState()
    {
        instance = null;
        OnDeath = delegate { };
    }
}
