using UnityEngine;
public class Projectile : MonoBehaviour
{
	[Header("Graphics")]
	[SerializeField] GameObject _impactParticle;
	[SerializeField] GameObject _projectileParticle;
	[SerializeField] GameObject _muzzleParticle;
	[SerializeField] GameObject[] _trailParticles;

	Target _target;
	Vector3 _startPosition;
	Vector3 _lastPosition;
	Vector3 _spinAxis = Vector3.up;
	bool _isSpinning;
	bool _useParabolicArc;
	bool _towerProjectile;
	float _speed = 10f;
	float _spinSpeed = 360f;
	float _maxArcHeight;
	float _damage;
	float _hitTime;
	float _startTime;

	public void Setup(Target target, float damage, bool towerProjectile, ProjectileSettings projectileSettings)
	{
		_target = target;
		_damage = damage;
		_towerProjectile = towerProjectile;

		_speed = projectileSettings.Speed;
		_isSpinning = projectileSettings.IsSpinning;
		_spinAxis = projectileSettings.SpinAxis;
		_spinSpeed = projectileSettings.SpinSpeed;
		_useParabolicArc = projectileSettings.UseParabolicArc;
		_maxArcHeight = projectileSettings.MaxArcHeight;

		_startPosition = transform.position;

		var initialDistance = Vector3.Distance(_startPosition, target.Center.position);
		_hitTime = initialDistance / _speed;
		_startTime = Time.time;
	}

	void Start()
	{
		_projectileParticle = Instantiate(_projectileParticle, transform.position, transform.rotation);
		_projectileParticle.transform.parent = transform;

		if (_muzzleParticle)
		{
			_muzzleParticle = Instantiate(_muzzleParticle, transform.position, transform.rotation);
			Destroy(_muzzleParticle, 1.5f);
		}
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

			if (!_isSpinning)
			{
				var rotation = Quaternion.LookRotation(arcPosition - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _speed * Time.deltaTime);
			}

			transform.position = arcPosition;
		}
		else
		{
			transform.position = Vector3.Lerp(_startPosition, _lastPosition, normalizedTime);

			if (!_isSpinning)
			{
				var rotation = Quaternion.LookRotation(_lastPosition - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _speed * Time.deltaTime);
			}
		}

		if (_isSpinning)
		{
			transform.Rotate(_spinAxis, _spinSpeed * Time.deltaTime);
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
			}
		}

		DestroyProjectile();
	}

	void DestroyProjectile()
	{
		if (_impactParticle)
		{
			_impactParticle = Instantiate(_impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, -transform.forward));
			Destroy(_impactParticle, 5.0f);
		}

		Destroy(_projectileParticle, 3f);
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
}
