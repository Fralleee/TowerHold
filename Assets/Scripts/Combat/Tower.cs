using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tower : Target
{
	public static Action<int, int> OnHealthChanged = delegate { };
	public static Action OnTowerDeath = delegate { };

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
		OnHealthChanged(Health, MaxHealth);
		OnDamageTaken += HandleDamageTaken;
	}

	protected override void Start()
	{
		base.Start();

		StartCoroutine(RegenerateHealth());
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

	void HandleDamageTaken(int damage)
	{
		ScoreManager.Instance.DamageTaken += damage;
		OnHealthChanged(Health, MaxHealth);
	}

	void OnDestroy()
	{
		OnTowerDeath();
		OnDamageTaken -= HandleDamageTaken;
	}

	public static void ResetGameState() => Instance = null;

	void OnDrawGizmosSelected()
	{
		var position = transform.position + (Vector3.up * 5f);
		var zoneColors = new Color[] { Color.green, Color.cyan, Color.blue, Color.yellow, Color.red };
		var zoneRanges = SpatialPartition.Zones.Values.ToArray();
		for (var i = 0; i < zoneRanges.Length; i++)
		{
			Gizmos.color = zoneColors[i % zoneColors.Length];
			GizmosExtras.Draw2dCircle(position, zoneRanges[i]);
		}
	}
}
