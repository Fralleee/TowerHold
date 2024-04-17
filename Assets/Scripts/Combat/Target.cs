using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class Target : MonoBehaviour
{
	public Action<Target> OnDeath = delegate { };
	public Action<int> OnDamageTaken = delegate { };
	public Action<int, int> OnHealthChanged = delegate { };

	[HideInInspector] public float Scale = 1f;
	public Transform Center;
	public int MaxHealth = 100;
	[ProgressBar(0, "MaxHealth", ColorGetter = "GetHealthBarColor")] public int Health;
	[SerializeField] protected HealthBar HealthBar;
	[SerializeField] float _healthBarOffset = 0f;
	[ReadOnly] public bool IsDead;
	public DamageModifiers DamageModifiers;

	[Header("Audio")]
	[SerializeField] AudioResource _deathSound;
	[SerializeField] protected AudioSettings AudioSettings;

	protected AudioSource AudioSource;

	public Dictionary<string, IDebuff> ActiveDebuffs;

	protected virtual void Awake()
	{
		if (Center == null)
		{
			Center = transform;
		}

		OnHealthChanged(Health, MaxHealth);
		AudioSource = GetComponent<AudioSource>();
		AudioSettings.ApplySettings(AudioSource);
		ActiveDebuffs = new Dictionary<string, IDebuff>();
		DamageModifiers = new DamageModifiers();
	}

	protected virtual void Start()
	{
		HealthBar = Instantiate(HealthBar, transform.position + (Vector3.up * _healthBarOffset), Quaternion.identity, transform);
	}

	protected virtual void Update()
	{
		// Use ToList() if you might modify the collection during iteration
		foreach (var debuff in ActiveDebuffs.Values.ToList())
		{
			debuff.Tick(this);
		}
	}

	public bool HasDebuff(string debuffName)
	{
		return ActiveDebuffs.ContainsKey(debuffName);
	}

	public float TakeDamage(int baseDamage)
	{
		if (IsDead)
		{
			return 0;
		}

		var damage = (int)(baseDamage * DamageModifiers.GetMultiplier());
		Health -= damage;
		HealthBar.SetHealth(Health);
		OnHealthChanged(Health, MaxHealth);

		if (Health <= 0)
		{
			Die();
			var actualDamage = damage + Health;
			OnDamageTaken(actualDamage);
			return actualDamage;
		}
		OnDamageTaken(damage);
		return baseDamage;

	}

	public void ApplyDebuff(IDebuff debuff)
	{
		if (HasDebuff(debuff.Identifier))
		{
			// Ensure your debuffs have a Refresh method to reset their timers or amounts
			ActiveDebuffs[debuff.Identifier].Refresh();
		}
		else
		{
			ActiveDebuffs.Add(debuff.Identifier, debuff);
			debuff.Apply(this);
		}
	}

	public void RemoveDebuff(IDebuff debuff)
	{
		if (ActiveDebuffs.ContainsKey(debuff.Identifier))
		{
			Debug.Log($"Removed debuff: {debuff.Identifier}");
			_ = ActiveDebuffs.Remove(debuff.Identifier);
		}
	}

	void PlaySound(AudioResource clip)
	{
		if (clip != null && AudioSource != null)
		{
			AudioSource.resource = clip;
			AudioSource.Play();
		}
		else
		{
			Debug.LogWarning("Target: No audio source or death sound assigned.", gameObject);
		}
	}

	public virtual void Die()
	{
		PlaySound(_deathSound);
		OnDeath(this);
		IsDead = true;
	}

	Color GetHealthBarColor(float value)
	{
		return Color.Lerp(Color.red, Color.green, Mathf.Pow(value / 100f, 2));
	}

	protected virtual void OnValidate()
	{
		Health = MaxHealth;
	}
}
