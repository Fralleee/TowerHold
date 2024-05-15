using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class Target : MonoBehaviour
{
	// Used to correctly size SelectionDecal as well as distance for projectile on larger objects
	[HideInInspector] public float Scale = 1f;

	public Transform Center;
	public int MaxHealth = 100;
	[ProgressBar(0, "MaxHealth", ColorGetter = "GetHealthBarColor")] public int Health;
	[SerializeField] protected HealthBar HealthBar;
	[SerializeField] float _healthBarOffset = 0f;
	[SerializeField] int _armor;
	[SerializeField] int _magicResistance;

	[ReadOnly] public bool IsDead;
	public DamageModifiers DamageModifiers;

	[Header("Audio")]
	[SerializeField] AudioResource _deathSound;

	protected AudioSource AudioSource;

	public Dictionary<string, IDebuff> ActiveDebuffs;

	float MitigationFactor(DamageType damageType)
	{
		if (damageType == DamageType.Global)
		{
			return 1000;
		}

		return 100 / (100 + (float)(damageType == DamageType.Physical ? _armor : _magicResistance));
	}

	protected virtual void Awake()
	{
		if (Center == null)
		{
			Center = transform;
		}

		EventBus<TargetHealthChangedEvent>.Raise(new TargetHealthChangedEvent { Target = this, Health = Health, MaxHealth = MaxHealth });
		AudioSource = GetComponent<AudioSource>();
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

	public virtual float TakeDamage(int baseDamage, DamageType damageType)
	{
		if (IsDead)
		{
			return 0;
		}

		var damage = (int)(baseDamage * DamageModifiers.GetMultiplier() * MitigationFactor(damageType));
		Health -= damage;
		HealthBar.SetHealth(Health);
		EventBus<TargetHealthChangedEvent>.Raise(new TargetHealthChangedEvent { Target = this, Health = Health, MaxHealth = MaxHealth });

		if (Health <= 0)
		{
			Die();
			var actualDamage = damage + Health;
			EventBus<TargetDamageTakenEvent>.Raise(new TargetDamageTakenEvent { Target = this, Damage = actualDamage });
			return actualDamage;
		}
		EventBus<TargetDamageTakenEvent>.Raise(new TargetDamageTakenEvent { Target = this, Damage = damage });
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
		IsDead = true;
		EventBus<TargetDeathEvent>.Raise(new TargetDeathEvent { Target = this });
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
