using UnityEngine;
public class Projectile : MonoBehaviour
{
	[Header("Graphics")]
	[SerializeField] GameObject _impactParticle;
	[SerializeField] GameObject _muzzleParticle;
	[SerializeField] GameObject[] _trails;

	[Header("Audio")]
	[SerializeField] AudioClip _attackSound;

	Turret _turret;
	Affliction _excludeBehavior;
	Target _target;
	DamageType _damageType;
	AudioSource _audioSource;
	Vector3 _startPosition;
	Vector3 _lastPosition;
	bool _useParabolicArc;
	bool _towerProjectile;
	bool _executeBehaviors;
	float _speed = 10f;
	float _maxArcHeight;
	float _damage;
	float _hitTime;
	float _startTime;

	public void Setup(Target target, Enemy enemy)
	{
		_target = target;
		_damage = enemy.Damage;
		_damageType = enemy.DamageType;
		_towerProjectile = false;
		_speed = enemy.ProjectileSettings.Speed;
		_useParabolicArc = enemy.ProjectileSettings.UseParabolicArc;
		_maxArcHeight = enemy.ProjectileSettings.MaxArcHeight;
		_startPosition = transform.position;

		// The distance is shorter if not a towerProjectile since we have to account for the towers's scale
		var initialDistance = _towerProjectile ? Vector3.Distance(_startPosition, target.Center.position) : Vector3.Distance(_startPosition, target.Center.position) - (Tower.Instance.Scale + 1f);
		_hitTime = initialDistance / _speed;
		_startTime = Time.time;

		_audioSource = GetComponent<AudioSource>();
	}

	public void Setup(Target target, Turret turret, ProjectileSettings projectileSettings, bool executeBehaviors = true, Affliction excludeBehavior = null)
	{
		_target = target;
		_damage = turret.BaseDamage;
		_damageType = turret.DamageType;
		_towerProjectile = true;
		_speed = projectileSettings.Speed;
		_useParabolicArc = projectileSettings.UseParabolicArc;
		_maxArcHeight = projectileSettings.MaxArcHeight;
		_turret = turret;
		_excludeBehavior = excludeBehavior;
		_executeBehaviors = executeBehaviors;
		_startPosition = transform.position;

		// The distance is shorter if not a towerProjectile since we have to account for the towers's scale
		var initialDistance = _towerProjectile ? Vector3.Distance(_startPosition, target.Center.position) : Vector3.Distance(_startPosition, target.Center.position) - (Tower.Instance.Scale + 1f);
		_hitTime = initialDistance / _speed;
		_startTime = Time.time;

		_audioSource = GetComponent<AudioSource>();
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
			var arcHeight = Mathf.Max(0f, (1f - Mathf.Pow((2f * normalizedTime) - 1f, 2f)) * _maxArcHeight);
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
			var actualDamage = _target.TakeDamage(Mathf.RoundToInt(_damage), _damageType);
			if (_towerProjectile)
			{
				if (_executeBehaviors)
				{
					_turret.AfflictionsController.TriggerAfflictions(_target, _turret, _excludeBehavior);
				}
				ScoreManager.Instance.DamageDone += actualDamage;
				(_target as Enemy).Attackers--;
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

		foreach (var trail in _trails)
		{
			trail.transform.SetParent(null);
			Destroy(trail, 2f);
		}

		Destroy(gameObject);
	}

	void PlaySound(AudioClip clip)
	{
		if (clip != null && _audioSource != null)
		{
			_audioSource.PlayOneShot(clip);
		}
		else
		{
			Debug.LogWarning("Projectile: No audio source or attack sound assigned.", gameObject);
		}
	}
}
