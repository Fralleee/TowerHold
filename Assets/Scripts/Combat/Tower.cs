using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Target
{
	public int HealthRegenerationRate = 5;

	public static Tower Instance;
	public List<Turret> Turrets;
	public Dictionary<DamageType, float> DamageMultipliers = new Dictionary<DamageType, float>() {
		{ DamageType.Normal, 1f },
		{ DamageType.Siege, 1f },
		{ DamageType.Technology, 1f },
		{ DamageType.Arcane, 1f },
		{ DamageType.Void, 1f }
	};

	protected override void Awake()
	{
		base.Awake();

		Instance = this;
	}

	protected override void Start()
	{
		base.Start();

		StartCoroutine(RegenerateHealth());
	}


	void Update()
	{
		_ = RegenerateHealth();
		foreach (var turret in Turrets)
		{
			turret.Update();
		}
	}

	IEnumerator RegenerateHealth()
	{
		while (true) // Creates an infinite loop, so the coroutine keeps running
		{
			// Increment health, ensuring that it doesn't exceed the maximum
			Health = Mathf.Min(Health + HealthRegenerationRate, MaxHealth);
			HealthBar.SetHealth(Health, true);

			// You may want to add a callback or event when the health changes, for UI updates or other game logic.

			yield return new WaitForSeconds(1); // Wait for 1 second before the loop continues
		}
	}

	public void AddTurret(Turret turret)
	{
		var instance = Instantiate(turret);
		instance.Setup(this);
		Turrets.Add(instance);
	}

	public void AddUppgrade(DamageType damageType) => DamageMultipliers[damageType] += 0.1f;

	public float GetDamage(DamageType damageType, float damage) => damage * DamageMultipliers[damageType];

	public void UpgradeHealth(int amount)
	{
		MaxHealth += amount;
		Health += amount;
		HealthBar.SetMaxHealth(MaxHealth);
		HealthBar.SetHealth(Health, true);
	}

	void HandleDamageTaken(int damage) => ScoreManager.Instance.DamageTaken += damage;

	void OnEnable() => OnDamageTaken += HandleDamageTaken;

	void OnDestroy() => OnDamageTaken -= HandleDamageTaken;

	public static void ResetGameState() => Instance = null;
}
