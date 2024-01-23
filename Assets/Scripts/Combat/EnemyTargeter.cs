using UnityEngine;

public class EnemyTargeter : MonoBehaviour, ITargeter
{
	public float CheckDistanceInterval = 1f;
	float _lastDistanceCheck = 0f;
	float _maxRange = 0;

	Target _target;

	void FixedUpdate()
	{
		if (_target == null && Time.time - _lastDistanceCheck > CheckDistanceInterval)
		{
			_lastDistanceCheck = Time.time;
			CheckDistance();
		}
	}

	void CheckDistance()
	{
		var distanceToTarget = Vector3.Distance(transform.position, Tower.Instance.transform.position);
		if (distanceToTarget < _maxRange)
		{
			_target = Tower.Instance;
		}
	}

	public Target GetTarget(float range)
	{
		_maxRange = range;
		return _target;
	}
}
