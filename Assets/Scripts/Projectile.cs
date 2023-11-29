using UnityEngine;
public class Projectile : MonoBehaviour
{
	[Header("Graphics")]
	[SerializeField] GameObject _impactParticle;
	[SerializeField] GameObject _projectileParticle;
	[SerializeField] GameObject _muzzleParticle;
	[SerializeField] GameObject[] _trailParticles;

	float _speed = 10f;
	bool _rotateTowardsTarget;
	bool _isSpinning;
	Vector3 _spinAxis = Vector3.up; // Default spin axis
	float _spinSpeed = 360f; // Degrees per second
	bool _useGravity;

	Target _target;
	float _damage = 10f;
	bool _towerProjectile;
	Vector3 _targetLastPosition = Vector3.zero;
	Vector3 _velocity;

	public void Setup(Target target, float damage, bool towerProjectile, ProjectileSettings projectileSettings)
	{
		_target = target;
		_damage = damage;
		_towerProjectile = towerProjectile;

		_speed = projectileSettings.Speed;
		_rotateTowardsTarget = projectileSettings.RotateTowardsTarget;
		_isSpinning = projectileSettings.IsSpinning;
		_spinAxis = projectileSettings.SpinAxis;
		_spinSpeed = projectileSettings.SpinSpeed;
		_useGravity = projectileSettings.UseGravity;

		if (_useGravity)
		{
			_targetLastPosition = _target.Center.position;
			CalculateTrajectory();
		}
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
			_targetLastPosition = _target.Center.position;
			if (_useGravity)
			{
				CalculateTrajectory();  // Recalculate if target moves
			}
		}

		if (_useGravity)
		{
			_velocity.y += Physics.gravity.y * Time.deltaTime;
			transform.position += _velocity * Time.deltaTime;
			if (_rotateTowardsTarget)
			{
				transform.LookAt(transform.position + _velocity);
			}
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, _targetLastPosition, _speed * Time.deltaTime);
			if (_rotateTowardsTarget)
			{
				transform.LookAt(_targetLastPosition);
			}
		}

		if (_isSpinning)
		{
			transform.Rotate(_spinAxis, _spinSpeed * Time.deltaTime);
		}

		if (Vector3.Distance(transform.position, _targetLastPosition) < 0.5f)
		{
			HitTarget();
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

	void CalculateTrajectory()
	{
		var distanceToTarget = Vector3.Distance(transform.position, _targetLastPosition);
		var yOffset = _targetLastPosition.y - transform.position.y;

		var toTarget = _targetLastPosition - transform.position;
		toTarget.y = 0;

		var time = distanceToTarget / _speed;
		_velocity = toTarget.normalized * _speed;

		_velocity.y = (yOffset / time) + (0.5f * Mathf.Abs(Physics.gravity.y) * time);

		if (_rotateTowardsTarget)
		{
			transform.LookAt(_targetLastPosition);
		}
	}
}
