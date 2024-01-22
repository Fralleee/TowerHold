using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Target : MonoBehaviour
{
	public Action<Target> OnDeath = delegate { };
	public Action<int> OnDamageTaken = delegate { };
	public Transform Center;
	public int MaxHealth = 100;
	public int Health = 100;
	[SerializeField] protected HealthBar HealthBar;
	[SerializeField] float _healthBarOffset = 0f;
	[ReadOnly] public bool IsDead;

	[Header("Audio")]
	[SerializeField] AudioClip _hitSound;
	[SerializeField] AudioSettings _audioSettings;

	AudioSource _audioSource;

	protected virtual void Awake()
	{
		if (Center == null)
		{
			Center = transform;
		}

		_audioSource = GetComponent<AudioSource>();
	}

	protected virtual void Start() => HealthBar = Instantiate(HealthBar, transform.position + (Vector3.up * _healthBarOffset), Quaternion.identity, transform);

	public float TakeDamage(int damage)
	{
		if (IsDead)
		{
			return 0;
		}

		Health -= damage;
		HealthBar.SetHealth(Health);
		PlaySound(_hitSound);

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

	void PlaySound(AudioClip clip)
	{
		if (clip != null && _audioSource != null)
		{
			_audioSettings.ApplySettings(_audioSource);
			_audioSource.PlayOneShot(clip);
		}
		else
		{
			Debug.LogWarning("Projectile: No audio source or attack sound assigned to projectile.", gameObject);
		}
	}

	public virtual void Die()
	{
		OnDeath(this);
		IsDead = true;
	}
}
