using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Target
{
    public int HealthRegenerationRate = 5;

    public static Tower instance;
    public List<Turret> turrets;
    public Dictionary<Category, float> damageMultipliers = new Dictionary<Category, float>() {
        { Category.Normal, 1f },
        { Category.Piercing, 1f },
        { Category.Siege, 1f },
        { Category.Magic, 1f },
        { Category.Chaos, 1f }
    };

    protected override void Awake()
    {
        base.Awake();

        instance = this;
    }

    protected override void Start()
    {
        base.Start();

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
            Health = Mathf.Min(Health + HealthRegenerationRate, MaxHealth);
            _healthBar.SetHealth(Health);

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

    public void AddUppgrade(Category category)
    {
        damageMultipliers[category] += 0.1f;
    }

    public float GetDamage(Category category, float damage)
    {
        return damage * damageMultipliers[category];
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
        OnAnyDeath = delegate { };
    }
}
