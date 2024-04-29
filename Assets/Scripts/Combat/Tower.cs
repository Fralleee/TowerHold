using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tower : Target
{
	public static new Action<int, int> OnHealthChanged = delegate { };
	public static Action OnTowerDeath = delegate { };
	public static Action<ShopItem> OnUpgrade = delegate { };

	public int HealthRegenerationRate = 5;

	public static Tower Instance;
	public List<Turret> Turrets;
	public float GlobalDamageMultiplier = 1f;
	public Dictionary<ShopType, float> ShopTypeMultipliers = new Dictionary<ShopType, float>() {
		{ ShopType.Force, 1f },
		{ ShopType.Precision, 1f },
		{ ShopType.Technology, 1f },
		{ ShopType.Arcane, 1f },
		{ ShopType.Chemical, 1f }
	};
	public Dictionary<DamageType, float> DamageTypeMultipliers = new Dictionary<DamageType, float>() {
		{ DamageType.Physical, 1f },
		{ DamageType.Magical, 1f },
		{ DamageType.Global, 1f },
	};

	protected override void Awake()
	{
		base.Awake();

		Scale = 4f;
		Instance = this;
		OnDamageTaken += HandleDamageTaken;
	}

	protected override void Start()
	{
		base.Start();

		_ = StartCoroutine(RegenerateHealth());
	}


	void FixedUpdate()
	{
		foreach (var turret in Turrets)
		{
			turret.FixedUpdate();
		}
	}

	IEnumerator RegenerateHealth()
	{
		while (true) // Creates an infinite loop, so the coroutine keeps running
		{
			// Increment health, ensuring that it doesn't exceed the maximum
			Health = Mathf.Min(Health + HealthRegenerationRate, MaxHealth);
			HealthBar.SetHealth(Health, true);
			OnHealthChanged(Health, MaxHealth);

			// You may want to add a callback or event when the health changes, for UI updates or other game logic.

			yield return new WaitForSeconds(1); // Wait for 1 second before the loop continues
		}
	}

	public void AddTurret(Turret turret)
	{
		var instance = Instantiate(turret);
		instance.Setup(this);
		instance.name = turret.name;
		Turrets.Add(instance);
	}

	public void AddUppgrade(DamageUpgrade damageUpgrade)
	{
		if (damageUpgrade.ShopType == ShopType.Offense)
		{
			if (damageUpgrade.DamageType == DamageType.Global)
			{
				GlobalDamageMultiplier += damageUpgrade.Amount;
			}
			else
			{
				DamageTypeMultipliers[damageUpgrade.DamageType] += damageUpgrade.Amount;
			}
		}
		else
		{
			ShopTypeMultipliers[damageUpgrade.ShopType] += damageUpgrade.Amount;
		}
	}

	public float GetDamage(DamageType damageType, ShopType shopType, float damage, float criticalHitChance, float criticalHitMultiplier)
	{
		var isCriticalHit = UnityEngine.Random.value < criticalHitChance;
		var multipliers = DamageTypeMultipliers[damageType] * ShopTypeMultipliers[shopType] * GlobalDamageMultiplier;
		var calculatedDamage = damage * (isCriticalHit ? criticalHitMultiplier : 1f) * multipliers;

		return calculatedDamage;
	}

	public void UpgradeHealth(int amount)
	{
		MaxHealth += amount;
		Health += amount;
		HealthBar.SetMaxHealth(MaxHealth);
		HealthBar.SetHealth(Health, true);
	}

	void HandleDamageTaken(int damage)
	{
		ScoreManager.Instance.DamageTaken += damage;
	}

	void OnDestroy()
	{
		OnTowerDeath();
		OnDamageTaken -= HandleDamageTaken;
	}

	public static void ResetGameState()
	{
		Instance = null;
	}
}
