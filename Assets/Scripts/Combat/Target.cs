using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

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

	[Header("Audio")]
	[SerializeField] AudioClip _deathSound;
	[SerializeField] protected AudioSettings AudioSettings;

	protected AudioSource AudioSource;

	public Dictionary<string, IDebuff> ActiveDebuffs = new Dictionary<string, IDebuff>();

	protected virtual void Awake()
	{
		if (Center == null)
		{
			Center = transform;
		}

		OnHealthChanged(Health, MaxHealth);
		AudioSource = GetComponent<AudioSource>();
		AudioSettings.ApplySettings(AudioSource);
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

	public float TakeDamage(int damage)
	{
		if (IsDead)
		{
			return 0;
		}

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
		return damage;

	}

	public void ApplyDebuff(IDebuff debuff)
	{
		if (ActiveDebuffs.ContainsKey(debuff.Identifier))
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
			_ = ActiveDebuffs.Remove(debuff.Identifier);
		}
	}

	void PlaySound(AudioClip clip)
	{
		if (clip != null && AudioSource != null)
		{
			AudioSource.PlayOneShot(clip);
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
