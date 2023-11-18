using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Target
{
	public int HealthRegenerationRate = 5;

	public static Tower Instance;
	public List<Turret> Turrets;
	public Dictionary<Category, float> DamageMultipliers = new Dictionary<Category, float>() {
		{ Category.Normal, 1f },
		{ Category.Piercing, 1f },
		{ Category.Siege, 1f },
		{ Category.Magic, 1f },
		{ Category.Chaos, 1f }
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
			HealthBar.SetHealth(Health);

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

	public void AddUppgrade(Category category) => DamageMultipliers[category] += 0.1f;

	public float GetDamage(Category category, float damage) => damage * DamageMultipliers[category];

	public void UpgradeHealth(int amount)
	{
		MaxHealth += amount;
		Health += amount;
		HealthBar.SetMaxHealth(MaxHealth);
		HealthBar.SetHealth(Health);
	}

	void HandleDamageTaken(int damage) => ScoreManager.Instance.DamageTaken += damage;

	void OnEnable() => OnDamageTaken += HandleDamageTaken;

	void OnDestroy() => OnDamageTaken -= HandleDamageTaken;

	public static void ResetGameState() => Instance = null;
}
