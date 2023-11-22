using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] float _speed = 10f;
	[SerializeField] bool _rotateTowardsTarget;
	[SerializeField] bool _isSpinning;
	[SerializeField] Vector3 _spinAxis = Vector3.up; // Default spin axis
	[SerializeField] float _spinSpeed = 360f; // Degrees per second
	[SerializeField] bool _useGravity;


	Target _target;
	float _damage = 10f;
	bool _towerProjectile;
	Vector3 _targetLastPosition = Vector3.zero;
	Vector3 _velocity;

	public void Setup(Target target, float damage, bool towerProjectile)
	{
		_target = target;
		_damage = damage;
		_towerProjectile = towerProjectile;

		if (_useGravity)
		{
			_targetLastPosition = _target.Center.position;
			CalculateTrajectory();
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
			Debug.Log("Spinning");
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
		Destroy(gameObject);
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
