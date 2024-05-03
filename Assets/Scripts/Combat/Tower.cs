using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Target
{
	public int HealthRegenerationRate = 5;

	public static Tower Instance;
	public List<Turret> Turrets;

	readonly DamageMultipliers _damageMultipliers = new DamageMultipliers();

	EventBinding<TargetDamageTakenEvent> _targetDamageTakenEvent;

	void OnEnable()
	{
		_targetDamageTakenEvent = new EventBinding<TargetDamageTakenEvent>(HandleDamageTaken);
		EventBus<TargetDamageTakenEvent>.Register(_targetDamageTakenEvent);
	}

	void OnDisable()
	{
		EventBus<TargetDamageTakenEvent>.Deregister(_targetDamageTakenEvent);
	}

	protected override void Awake()
	{
		base.Awake();

		Scale = 4f;
		Instance = this;
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
			EventBus<TowerHealthChangedEvent>.Raise(new TowerHealthChangedEvent { Health = Health, MaxHealth = MaxHealth });

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
		_damageMultipliers.AddUppgrade(damageUpgrade);
	}

	public float GetDamage(Turret turret)
	{
		var isCriticalHit = Random.value < turret.CriticalHitChance;
		var calculatedDamage = turret.BaseDamage * (isCriticalHit ? turret.CriticalHitMultiplier : 1f) * _damageMultipliers.GetMultiplier(turret.DamageType, turret.ShopType);

		return calculatedDamage;
	}

	public void UpgradeHealth(int amount)
	{
		MaxHealth += amount;
		Health += amount;
		HealthBar.SetMaxHealth(MaxHealth);
		HealthBar.SetHealth(Health, true);
	}

	void HandleDamageTaken(TargetDamageTakenEvent e)
	{
		ScoreManager.Instance.DamageDone += e.Damage;
	}

	void OnDestroy()
	{
		EventBus<TowerDeathEvent>.Raise(new TowerDeathEvent());
	}

	public static void ResetGameState()
	{
		Instance = null;
	}
}
