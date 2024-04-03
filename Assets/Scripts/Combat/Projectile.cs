using UnityEngine;
public class Projectile : MonoBehaviour
{
	[Header("Graphics")]
	[SerializeField] GameObject _impactParticle;
	[SerializeField] GameObject _muzzleParticle;

	[Header("Audio")]
	[SerializeField] AudioClip _attackSound;
	[SerializeField] AudioSettings _audioSettings;

	Target _target;
	AudioSource _audioSource;
	Vector3 _startPosition;
	Vector3 _lastPosition;
	bool _useParabolicArc;
	bool _towerProjectile;
	float _speed = 10f;
	float _maxArcHeight;
	float _damage;
	float _hitTime;
	float _startTime;

	bool _isDamageOverTime;
	float _dotDuration = 5f;
	float _dotTotalDamage = 10f;
	float _dotTickRate = 1f;

	public void Setup(Target target, float damage, bool towerProjectile, ProjectileSettings projectileSettings)
	{
		_target = target;
		_damage = damage;
		_towerProjectile = towerProjectile;
		_speed = projectileSettings.Speed;
		_useParabolicArc = projectileSettings.UseParabolicArc;
		_maxArcHeight = projectileSettings.MaxArcHeight;
		_startPosition = transform.position;

		// The distance is shorter if not a towerProjectile since we have to account for the towers's scale
		var initialDistance = _towerProjectile ? Vector3.Distance(_startPosition, target.Center.position) : Vector3.Distance(_startPosition, target.Center.position) - (Tower.Instance.Scale + 1f);
		_hitTime = initialDistance / _speed;
		_startTime = Time.time;

		_audioSource = GetComponent<AudioSource>();
	}

	public void SetupDamageOverTime(float dotDuration, float dotTotalDamage, float dotTickRate)
	{
		_isDamageOverTime = true;
		_dotDuration = dotDuration;
		_dotTotalDamage = dotTotalDamage;
		_dotTickRate = dotTickRate;
	}


	void Start()
	{
		if (_muzzleParticle)
		{
			_muzzleParticle = Instantiate(_muzzleParticle, transform.position, transform.rotation);
			Destroy(_muzzleParticle, 1.5f);
		}

		PlaySound(_attackSound);
	}

	void Update()
	{
		if (_target != null)
		{
			_lastPosition = _target.Center.position;
		}

		var timeElapsed = Time.time - _startTime;
		if (timeElapsed > _hitTime)
		{
			HitTarget();
			return;
		}


		var normalizedTime = timeElapsed / _hitTime;

		if (_useParabolicArc)
		{
			var arcHeight = Mathf.Max(0f, (1f - Mathf.Pow(2f * normalizedTime - 1f, 2f)) * _maxArcHeight);
			var arcPosition = Vector3.Lerp(_startPosition, _lastPosition, normalizedTime);
			arcPosition.y += arcHeight;

			var rotation = Quaternion.LookRotation(arcPosition - transform.position);
			transform.SetPositionAndRotation(arcPosition, Quaternion.Slerp(transform.rotation, rotation, _speed * Time.deltaTime));
		}
		else
		{
			transform.position = Vector3.Lerp(_startPosition, _lastPosition, normalizedTime);

			var rotation = Quaternion.LookRotation(_lastPosition - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _speed * Time.deltaTime);
		}
	}

	void HitTarget()
	{
		if (_target != null)
		{
			var actualDamage = _target.TakeDamage(Mathf.RoundToInt(_damage));
			if (_towerProjectile)
			{
				ScoreManager.Instance.DamageDone += actualDamage;
				(_target as Enemy).Attackers--;
			}

			if (_isDamageOverTime)
			{
				_target.ApplyDebuff(new DamageOverTimeDebuff(name, _dotDuration, _dotTotalDamage, _dotTickRate, _impactParticle));
			}
		}

		DestroyProjectile();
	}

	void DestroyProjectile()
	{
		if (_impactParticle)
		{
			var position = _towerProjectile ? transform.position : transform.position - (transform.forward * (Tower.Instance.Scale + 1f));
			_impactParticle = Instantiate(_impactParticle, position, Quaternion.FromToRotation(Vector3.up, -transform.forward));
			Destroy(_impactParticle, 5.0f);
		}

		Destroy(gameObject);
		var trails = GetComponentsInChildren<ParticleSystem>();
		for (var i = 1; i < trails.Length; i++)
		{
			var trail = trails[i];
			if (trail.gameObject.name.Contains("Trail"))
			{
				trail.transform.SetParent(null);
				Destroy(trail.gameObject, 2f);
			}
		}
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
			Debug.LogWarning("Projectile: No audio source or attack sound assigned.", gameObject);
		}
	}
}
