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
	Vector3 _spinAxis = Vector3.up;
	bool _isSpinning;
	bool _useTrajectory;
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
		_useTrajectory = projectileSettings.UseTrajectory;
		_maxArcHeight = projectileSettings.ArcHeight;

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
		if (_target == null)
		{
			return;
		}

		var timeElapsed = Time.time - _startTime;
		if (timeElapsed > _hitTime)
		{
			HitTarget();
			return;
		}


		var normalizedTime = timeElapsed / _hitTime;
		var targetPosition = _target.Center.position;

		if (_useTrajectory)
		{
			var arc = Mathf.Sin(normalizedTime * Mathf.PI) * _maxArcHeight;
			var arcPosition = Vector3.Lerp(_startPosition, targetPosition, normalizedTime);
			arcPosition.y += arc;

			if (!_isSpinning)
			{
				var rotation = Quaternion.LookRotation(arcPosition - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _speed * Time.deltaTime);
			}

			transform.position = arcPosition;
		}
		else
		{
			transform.position = Vector3.Lerp(_startPosition, targetPosition, normalizedTime);
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
		_impactParticle = Instantiate(_impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, -transform.forward));
		foreach (var trail in _trailParticles)
		{
			var curTrail = transform.Find(_projectileParticle.name + "/" + trail.name).gameObject;
			curTrail.transform.parent = null;
			Destroy(curTrail, 3f);
		}
		Destroy(_projectileParticle, 3f);
		Destroy(_impactParticle, 5.0f);
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
